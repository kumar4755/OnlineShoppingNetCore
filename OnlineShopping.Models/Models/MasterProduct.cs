using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class MasterProduct
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SubCategoryId { get; set; }
        public int ChildSubCategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> CreatedOnUtc { get; set; }
        public Nullable<DateTime> UpdatedOnUtc { get; set; }
    }
}
