using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public System.Guid CustomerGuid { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Nullable<int> PasswordFormatId { get; set; }
        public string PasswordSalt { get; set; }
        public string AdminComment { get; set; }
        public Nullable<bool> IsTaxExempt { get; set; }
        public Nullable<int> AffiliateId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<bool> HasShoppingCartItems { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<bool> IsSystemAccount { get; set; }
        public string SystemName { get; set; }
        public string LastIpAddress { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<System.DateTime> LastLoginDateUtc { get; set; }
        public System.DateTime LastActivityDateUtc { get; set; }
        public Nullable<int> BillingAddress_Id { get; set; }
        public Nullable<int> ShippingAddress_Id { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string AboutYou { get; set; }
        public string Personalize { get; set; }
        public Nullable<int> Subscription_Id { get; set; }
        public string Subscription_Payment_Id { get; set; }
        public Nullable<decimal> WalletAmt { get; set; }
        public string PicturePath { get; set; }
        public string SuspensionReason { get; set; }
        public Nullable<System.DateTime> UpdatedOnUtc { get; set; }
    }
}
