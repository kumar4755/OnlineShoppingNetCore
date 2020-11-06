using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;

namespace OnlineShopping.FrontEnd.Repository
{
    public class CategoryRepository : SqlRepository, ICategoryRepository
    {
        public CategoryRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<CategoryVM> GetAllCategories()
        {
            using (var conn = GetOpenConnection())
            {
                string query = @"SELECT CC.Id, CC.NAME, CC.ParentCategoryId AS ParentCategoryId,CP.Name AS ParentCategory, CC.IncludeInTopMenu AS IncludeInTopMenu,  " +
       " CC.DisplayOrder AS DisplayOrder FROM Category CC " +
       " LEFT JOIN Category CP ON CP.Id = CC.ParentCategoryId " +
       " WHERE CC.Deleted = 0 ORDER BY CC.ParentCategoryId ASC;";

                var categoriesList = conn.Query<Category>(query).ToList();

                var list = (from s in categoriesList where s.ParentCategoryId == 0 select s).Select(z => new CategoryVM
                {
                    CategoryId = z.Id,
                    CategoryName = z.Name,
                    SubCategoryList = (from a in categoriesList where a.ParentCategoryId == z.Id select a).Select(b => new SubCategoryVM
                    {
                        SubCategoryId = b.Id,
                        SubCategoryName = b.Name,
                        ChildSubCategoryList = (from a in categoriesList where a.ParentCategoryId == b.Id select a).Select(a => new ChildSubCategoryVM
                        {
                            ChildSubCategoryId = a.Id,
                            ChildSubCategoryName = a.Name,
                        }).ToList()
                    }).ToList()
                });

                return list;
            }
        }

    }
}
