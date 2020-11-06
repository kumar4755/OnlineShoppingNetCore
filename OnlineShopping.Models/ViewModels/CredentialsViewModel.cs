using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;


namespace OnlineShopping.Models.ViewModels
{
    [Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
