using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.FrontEnd.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int ProductId);
        IEnumerable<ProductsVM> GetAllProductsByCategory(object obj);
    }
}
