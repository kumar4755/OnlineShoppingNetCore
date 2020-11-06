using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnlineShopping.FrontEnd.Api.Data;
using AutoMapper;
using OnlineShopping.FrontEnd.Api.Models.Entities;
using OnlineShopping.Models.ViewModels;
using OnlineShopping.FrontEnd.Api.Helpers;
using OnlineShopping.FrontEnd.Repository;
using System.Net;
using OnlineShopping.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using OnlineShopping.Email;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineShopping.FrontEnd.Api.Controllers
{
    [Route("api/Accounts")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository = null;
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private IEmailSender _emailSender = null;


        public AccountsController(UserManager<AppUser> userManager, IMapper mapper, ApplicationDbContext appDbContext, ICustomerRepository customerRepository, IHostingEnvironment hostingEnvironment, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _customerRepository = customerRepository;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        // POST api/accounts
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Email = model.Email.Trim().Replace(" ", "");
            var customerEmail = _customerRepository.ReadCustomerEmail(model.Email);

            IActionResult errorResult = null;

            if (customerEmail != null)
            {
                ModelState.AddModelError("", "Email already registered");
                errorResult = BadRequest(ModelState);
            }
            else
            {
                var userIdentity = _mapper.Map<AppUser>(model);

                var result = await _userManager.CreateAsync(userIdentity, model.Password);

                if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

                await _appDbContext.JobSeekers.AddAsync(new JobSeeker { IdentityId = userIdentity.Id, Location = model.Location });
                await _appDbContext.SaveChangesAsync();
                errorResult = GetErrorResult(result);
            }

            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                registerCustomerAccount(model);
                var customer = _customerRepository.ReadCustomerEmail(model.Email);
                _customerRepository.UpdateForgotPassword(model.Email);
                var ForgotPasswordDetails = _customerRepository.ReadForgotPasswordDetails(model.Email);

                var mailBody = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "Content/mail_templates/activateregistration.html"));
                mailBody = mailBody.Replace("{CustomerName}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(customer.Username));
                mailBody = mailBody.Replace("{Email}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(customer.Email).ToLower());
                mailBody = mailBody.Replace("{activateregistrationlink}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_configuration.GetSection("MyConfigSettings").GetSection("FrontEndBaseUri").Value + "#/activateregistration?guid=" + ForgotPasswordDetails.CustomerGuid).ToLower());
                string Subject = "Account Activation";

                SendRegistrationLinkEmail(customer.Username, customer.Email, Subject, mailBody);
            }
            return new OkObjectResult("Account created");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ActivateRegistration")]
        public async Task<IActionResult> ActivateRegistration(string guid)
        {
            IActionResult errorResult = null;
            var activateRegistrationDetails = _customerRepository.ReadEmailActivationDetails(guid);
            if (activateRegistrationDetails == null || activateRegistrationDetails.IsExpired == true)
            {
                ModelState.AddModelError("", "Sorry this has been link expired");
                errorResult = BadRequest(ModelState);
            }

            if (errorResult != null)
            {
                return errorResult;
            }
            else
            {
                if (activateRegistrationDetails != null)
                {
                    var customer = _customerRepository.ReadCustomerEmail(activateRegistrationDetails.Email);
                    if (customer != null)
                    {
                        _customerRepository.UpdateCustomerAccountStatus(customer.Id);
                        _customerRepository.UpdateForgotPasswordDetails(activateRegistrationDetails.Id);
                    }
                }
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EmailValidate")]
        public async Task<IActionResult> ForgotPasswordEmailValidate(string email)
        {
            try
            {
                var customerEmail = _customerRepository.ReadCustomerEmail(email);

                if (customerEmail != null)
                {
                    _customerRepository.UpdateForgotPassword(email);
                    var ForgotPasswordDetails = _customerRepository.ReadForgotPasswordDetails(email);
                    var mailBody = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "Content/mail_templates/forgotpassword.html"));
                    mailBody = mailBody.Replace("{CustomerName}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(customerEmail.Username));
                    mailBody = mailBody.Replace("{Email}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(email).ToLower());
                    mailBody = mailBody.Replace("{forgotpasswordlink}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_configuration.GetSection("MyConfigSettings").GetSection("FrontEndBaseUri").Value + "#/forgotpassword?Email=" + ForgotPasswordDetails.Email + "&guid=" + ForgotPasswordDetails.CustomerGuid + "&username=" + customerEmail.Username + "&createddate=" + ForgotPasswordDetails.CreatedOnUtc + "&isexpired=" + ForgotPasswordDetails.IsExpired + "&id=" + ForgotPasswordDetails.Id));
                    string Subject = "Change your password";
                    string FullName = customerEmail.Username + " " + customerEmail.LastName;

                    SendRegistrationLinkEmail(FullName, customerEmail.Email, Subject, mailBody);
                }
                else
                {
                    ModelState.AddModelError("", "Email does'nt exist");
                    IActionResult errorResult = BadRequest(ModelState);
                    if (errorResult != null)
                    {
                        return errorResult;
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("UpdateForgotPassword")]
        public async Task<IActionResult> UpdateForgotPassword(string username, string password, int id)
        {
            try
            {
                CredentialsViewModel userModel = new CredentialsViewModel
                {
                    UserName = username,
                    Password = password
                };

                IActionResult errorResult = null;
                var forgotPwdDetails = _customerRepository.ReadForgotPasswordDetails(id);
                if (forgotPwdDetails.IsExpired == true)
                {
                    ModelState.AddModelError("", "Sorry this has been link expired");
                    errorResult = BadRequest(ModelState);
                }
                else
                {
                    var userData = await _userManager.FindByNameAsync(userModel.UserName);
                    string resetToken = await _userManager.GeneratePasswordResetTokenAsync(userData);
                    IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(userData, resetToken, userModel.Password);

                    errorResult = GetErrorResult(passwordChangeResult);
                }

                if (errorResult != null)
                {
                    return errorResult;
                }
                else
                {
                    _customerRepository.UpdateForgotPasswordDetails(id);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [NonAction]
        private void registerCustomerAccount(RegistrationViewModel customerobj)
        {
            Customer customer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                Username = customerobj.FirstName,
                Email = customerobj.Email,
                Deleted = false,
                Active = false,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            _customerRepository.CreateCustomer(customer);
        }

        [NonAction]
        public void SendRegistrationLinkEmail(string userName, string EMail, string Subject, string EMailBody)
        {
            SendGridMessage msg = new SendGridMessage();
            List<EmailAddress> toList = new List<EmailAddress>();
            toList.Add(new EmailAddress(EMail, userName));
            msg.AddTos(toList);
            msg.Subject = Subject;
            msg.HtmlContent = EMailBody;
            _emailSender.Execute(msg);
        }

        #region Helpers

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return new ContentResult() { Content = "Sorrry could'nt create your account.", StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        #endregion
    }
}
