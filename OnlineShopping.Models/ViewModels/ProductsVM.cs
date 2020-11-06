using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class ProductsVM
    {
        public MasterProduct masterproduct { get; set; }
        public List<Product> productDetails { get; set; }
    }
}
