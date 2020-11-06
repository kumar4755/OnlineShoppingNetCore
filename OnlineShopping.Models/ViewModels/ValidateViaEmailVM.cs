using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class ValidateViaEmailVM
    {
        public int Id { get; set; }
        public System.Guid CustomerGuid { get; set; }
        public string Email { get; set; }
        public bool IsExpired { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<System.DateTime> UpdatedOnUtc { get; set; }
    }
}
