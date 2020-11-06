using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategoryVM> SubCategoryList { get; set; } = new List<SubCategoryVM>();
    }
    public class SubCategoryVM
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public List<ChildSubCategoryVM> ChildSubCategoryList { get; set; } = new List<ChildSubCategoryVM>();
    }
    public class ChildSubCategoryVM
    {
        public int ChildSubCategoryId { get; set; }
        public string ChildSubCategoryName { get; set; }
    }
}
