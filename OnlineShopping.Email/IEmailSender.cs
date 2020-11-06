using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Email
{
    public interface IEmailSender
    {
        Task Execute(SendGridMessage msg, EmailAddress fromAddress = null);
    }
}
