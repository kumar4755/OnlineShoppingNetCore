﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> StateProvinceId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string CustomAttributes { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public string Notes { get; set; }
        public bool Deleted { get; set; }
        public Nullable<DateTime> UpdatedOnUtc { get; set; }
        public string Landmark { get; set; }
        public bool AddressType { get; set; }
        public bool IsDefaultAddress { get; set; }
    }
}
