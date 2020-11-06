using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class CustomerAddressVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address1 { get; set; }
        public string ZipPostalCode { get; set; }
        public string Landmark { get; set; }
        public bool AddressType { get; set; }
        public bool IsDefaultAddress { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public string Notes { get; set; }
        public bool Deleted { get; set; }
        public string PhoneNumber { get; set; }
        public int Customer_Id { get; set; }
        public int Address_Id { get; set; }
    }
}
