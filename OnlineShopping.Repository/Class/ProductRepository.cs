using OnlineShopping.Infrastructure;
using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data;

namespace OnlineShopping.FrontEnd.Repository
{
    public class ProductRepository : SqlRepository, IProductRepository
    {
        public ProductRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<Product> GetAllProducts()
        {
            using (var conn = GetOpenConnection())
            {
                var sql = "SELECT * FROM Product";
                return conn.Query<Product>(sql);
            }
        }
        public Product GetProductById(int ProductId)
        {
            using (var conn = GetOpenConnection())
            {
                var sql = "SELECT * FROM Product where Id=@ProductId";
                return conn.QueryFirstOrDefault<Product>(sql, new
                {
                    ProductId
                });
            }
        }
        public IEnumerable<Product> GetAllProductsByMasterProductId(int MasterProductId)
        {
            IEnumerable<Product> _productsList = null;

            using (var conn = GetOpenConnection())
            {
                var sql = "SELECT * FROM Product where MasterProductId=@MasterProductId ORDER BY Price";
                _productsList = conn.Query<Product>(sql, new
                {
                    MasterProductId
                });
            }
            return _productsList;
        }
        public IEnumerable<ProductsVM> GetAllProductsByCategory(object obj)
        {
            JObject _jObj = JObject.FromObject(obj);
            List<ProductsVM> _prodList = new List<ProductsVM>();

            string searchText = Convert.ToString(_jObj["SearchText"]);
            int categoryId = Convert.ToInt32(_jObj["CategoryId"]);
            int pageNumber = Convert.ToInt32(_jObj["PageNumber"]);
            int noOfRows = Convert.ToInt32(_jObj["NoOfRows"]);
            bool PriceAsc = Convert.ToBoolean(_jObj["PriceAsc"]);

            using (var conn = GetOpenConnection())
            {

                var p = new DynamicParameters();
                p.Add("@SearchText ", searchText);
                p.Add("@CategoryId ", categoryId, dbType: DbType.Int32, direction: ParameterDirection.Input);
                p.Add("@PageNumber ", pageNumber, dbType: DbType.Int32, direction: ParameterDirection.Input);
                p.Add("@NoOfRows ", noOfRows, dbType: DbType.Int32, direction: ParameterDirection.Input);
                //p.Add("@PriceAsc ",PriceAsc, dbType: DbType.Boolean, direction: ParameterDirection.Input);

                //To Get the Master Product list from SP
                var MasterProductList = conn.Query<MasterProduct>("GetAllProductsByCategoryTemp", p, commandType: CommandType.StoredProcedure).ToList();

                foreach (var item in MasterProductList)
                {
                    List<Product> _productsList = new List<Product>();
                    _productsList = GetAllProductsByMasterProductId(item.Id).ToList();  // To Get the Productlist by passing MasterProductId

                    //If No Products are there for Master Product skip that record
                    if (_productsList.Count() > 0)
                    {
                        ProductsVM _productsVM = new ProductsVM()
                        {
                            masterproduct = item,
                            productDetails = _productsList
                        };
                        _prodList.Add(_productsVM);
                    }
                }

            }

            return _prodList;
        }
    }
}
