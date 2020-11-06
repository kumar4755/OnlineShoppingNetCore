using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Email
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Execute(SendGridMessage msg, EmailAddress fromAddress = null)
        {
            var apiKey = _configuration.GetSection("MyConfigSettings").GetSection("SendGridAPIKey").Value;
            var fromEmailAddress = _configuration.GetSection("MyConfigSettings").GetSection("FromEmailAddress").Value;
            var FromEmailName = _configuration.GetSection("MyConfigSettings").GetSection("FromEmailName").Value;

            var from = (fromAddress == null) ? (new EmailAddress(fromEmailAddress, FromEmailName)) : fromAddress;
            msg.From = from;

            var client = new SendGridClient(apiKey);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
