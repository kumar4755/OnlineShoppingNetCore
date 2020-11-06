using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class CustomerProfileVM
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<System.DateTime> LastLoginDateUtc { get; set; }
        public System.DateTime LastActivityDateUtc { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string AboutYou { get; set; }
        public string Personalize { get; set; }
        public Nullable<int> Subscription_Id { get; set; }
        public Nullable<decimal> WalletAmt { get; set; }
        public string Subscription_Payment_Id { get; set; }
        public string PicturePath { get; set; }
        public string SuspensionReason { get; set; }
    }

    public class CustomerDetails
    {
        public CustomerProfileVM CustomerProfile { get; set; }
        public IEnumerable<CustomerAddressVM> CustomerAddress { get; set; }
    }
}
