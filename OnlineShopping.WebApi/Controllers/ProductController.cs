using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Email;
using OnlineShopping.FrontEnd.Repository;
using SendGrid.Helpers.Mail;

namespace OnlineShopping.FrontEnd.Api.Controllers
{
    //[Authorize(Policy = "ApiUser")]
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository = null;
        private ILoggerManager _logger = null;
        private IEmailSender _emailSender = null;
        public ProductController(IProductRepository productRepository, ILoggerManager logger, IEmailSender emailSender)
        {
            _productRepository = productRepository;
            _logger = logger;
            _emailSender = emailSender;
        }

        [Route("GetAllProducts")]
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            //SendTestEMail(); //Send Test Email with SendGrid

            //_logger.LogInfo("Hi Kanth");
            //int a = 0, b = 1, c = 0;
            //c = b / a;
            var _productsList = _productRepository.GetAllProducts().ToList();
            return Ok(_productsList);
        }

        [Route("GetProductById/{ProductId}")]
        [HttpGet]
        public IActionResult GetProductById(int ProductId)
        {
            try
            {
                var _productsObj = _productRepository.GetProductById(ProductId);
                return Ok(_productsObj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetAllProductsByCategory")]
        [HttpPost]
        public IActionResult GetAllProductsByCategory(object obj)
        {
            try
            {
                var _productsList = _productRepository.GetAllProductsByCategory(obj);
                return Ok(_productsList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [NonAction]
        public void SendTestEMail()
        {
            SendGridMessage msg = new SendGridMessage();
            List<EmailAddress> toList = new List<EmailAddress>();
            toList.Add(new EmailAddress("lkshmknth@gmail.com", "lkshmknth"));
            msg.AddTos(toList);
            msg.Subject = "Sample Sendgrid Test Subject";
            msg.HtmlContent = "Sample Sendgrid Test Content";

            //string fileName = "Imgae";
            //string fileBase64 = "/9j/4AAQSkZJRgABAQEASABIAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAAYACwDASIAAhEBAxEB/8QAGQAAAwEBAQAAAAAAAAAAAAAAAAIGAwUE/8QAKhAAAQMDAgQFBQAAAAAAAAAAAQIDBAAFEQYhEjFBcRNCUWGBByKRsdH/xAAYAQEAAwEAAAAAAAAAAAAAAAAEAQIFA//EACURAAICAQIFBQEAAAAAAAAAAAECAAMREiEEBSKRwRMUMlFhof/aAAwDAQACEQMRAD8AvF6uvl5kKascIhseYpCiPck/aO1I5eNZ2lJenR/GZG5y2nAHdPL5q4ttuYtcFuLHSAhAxnG6j1J70rl0tyFKbcmxkqSSlSC6nIPUEZp3ua84SsY7nvCei2NTOc/yZ2W6C72xuYGFs8flX+weo966WM14mbjb1qS0zLjFR2ShDiST2Ga5d+1dAsTgZc4nZBGfCb5jv6UGxlXJOwjKK3tIRNz+ShoqJt/1HgSn0tSo7kXiOOMkKSO52I/FWaVeIkKQQUnkaojq/wAZ0votoOLFIMSNKZmRm32FBbbiQpKh1FTUrQVtmS3pLkiUFvOKcUEqTgEnJxtRRSQ7UuQhxC6RYOoSYhWqPH16xDgOrcaYUFKWsgnIGSMgDtWmjI7V41NcJ05IcdbVxJSvcAknfB9MYFFFTzMlrag2+0Xyk6KOI07Sj1xbIknTsiQttAeYTxIXjGDnGM+hzypNETJD+mI4VkhtRbST1A5fz4oorLs2sBH15mgnVwBLb4bxP//Z";
            //msg.AddAttachment(fileName, fileBase64);
            _emailSender.Execute(msg);
        }
    }
}