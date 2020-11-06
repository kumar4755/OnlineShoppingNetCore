using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.FrontEnd.Repository
{
    public interface ICategoryRepository
    {
        IEnumerable<CategoryVM> GetAllCategories();
    }
}
