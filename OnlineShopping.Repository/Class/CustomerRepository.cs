using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;

namespace OnlineShopping.FrontEnd.Repository
{
    public class CustomerRepository : SqlRepository, ICustomerRepository
    {
        public CustomerRepository(string connectionString) : base(connectionString) { }

        public Tuple<bool, string> AddOrUpdateToCart(CartVM cart)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");

            using (var conn = GetOpenConnection())
            {
                //Update
                if (cart.Id > 0)
                {
                    // Update Cart Items
                    if (!cart.IsDeleteItem)
                    {
                        conn.Execute("" +
                                  " UPDATE ShoppingCartItem SET Quantity = @Quantity, UpdatedOnUtc =  @UpdatedOnUtc " +
                                  " WHERE Id = @Id;",
                                  new
                                  {
                                      Id = cart.Id,
                                      ProductId = cart.ProductId,
                                      Quantity = cart.Quantity,
                                      UpdatedOnUtc = DateTime.UtcNow,
                                  });

                        tuple = new Tuple<bool, string>(true, "Item Updated Successfully");
                    }
                    //Delete from Cart Items
                    else
                    {
                        conn.Execute("" +
                                  " DELETE from ShoppingCartItem " +
                                  " WHERE Id = @Id;",
                                  new
                                  {
                                      Id = cart.Id,
                                  });

                        tuple = new Tuple<bool, string>(true, "Item Deleted Successfully");
                    }
                }
                //Add
                else
                {
                    var cartObj = conn.Query<CartVM>("SELECT * FROM ShoppingCartItem WHERE CustomerId=" + cart.CustomerId + "AND ProductId=" + cart.ProductId).FirstOrDefault();

                    //If Same Item already exists in Cart, update the existing record
                    if (cartObj != null)
                    {
                        conn.Execute
                                   (" UPDATE ShoppingCartItem SET Quantity = @Quantity, UpdatedOnUtc =  @UpdatedOnUtc " +
                                  " WHERE Id = @Id;",
                                       new
                                       {
                                           Id = cartObj.Id,
                                           CustomerId = cart.CustomerId,
                                           ProductId = cart.ProductId,
                                           Quantity = cartObj.Quantity + 1,
                                           UpdatedOnUtc = DateTime.UtcNow
                                       });
                    }
                    //If New Item
                    else
                    {
                        conn.Execute
                                   (" INSERT INTO ShoppingCartItem (CustomerId, ProductId, Quantity, CreatedOnUtc, UpdatedOnUtc) " +
                                           " VALUES (@CustomerId, @ProductId, @Quantity, @CreatedOnUtc, @UpdatedOnUtc)",
                                       new
                                       {
                                           CustomerId = cart.CustomerId,
                                           ProductId = cart.ProductId,
                                           Quantity = cart.Quantity,
                                           CreatedOnUtc = DateTime.UtcNow,
                                           UpdatedOnUtc = DateTime.UtcNow
                                       });
                    }

                    tuple = new Tuple<bool, string>(true, "Item Added Successfully");
                }

            }
            return tuple;
        }
        public int ReadCustomerId(string username)
        {
            var Username = username.Trim();
            int CustomerId = 0;
            using (var conn = GetOpenConnection())
            {
                CustomerId = conn.Query<int>(" SELECT Id FROM CUSTOMER WHERE Email = @username", new { Username }).FirstOrDefault();
            }
            return CustomerId;
        }
        public CustomerDetails ReadCustomerDetails(int Id)
        {
            CustomerDetails customerDetails = new CustomerDetails();
            customerDetails.CustomerProfile = ReadCustomerProfile(Id);
            customerDetails.CustomerAddress = ReadCustomerAddress(Id);
            return customerDetails;
        }
        public CustomerProfileVM ReadCustomerProfile(int customerId)
        {
            CustomerProfileVM _customerProfile = new CustomerProfileVM();
            using (var conn = GetOpenConnection())
            {
                _customerProfile = conn.Query<CustomerProfileVM>(" SELECT * FROM Customer WHERE Id =  @Id ",
                new
                {
                    Id = customerId
                }).SingleOrDefault();
            }
            return _customerProfile;
        }
        public IEnumerable<CustomerAddressVM> ReadCustomerAddress(int Id)
        {
            IEnumerable<CustomerAddressVM> _CustomerAddressList = null;

            using (var conn = GetOpenConnection())
            {
                _CustomerAddressList = conn.Query<CustomerAddressVM>(" SELECT CA.Customer_Id,CA.Address_Id,A.Id, A.FirstName, A.LastName, A.Company, A.City, A.Address1, A.ZipPostalCode, A.CreatedOnUtc, " +
                                          " A.Notes, A.PhoneNumber, A.Deleted FROM Address A INNER JOIN CustomerAddresses CA on CA.Address_Id = A.Id WHERE " +
                                          " CA.Customer_Id = @Id AND A.DELETED = 0 ",
                new
                {
                    Id
                });
            }
            return _CustomerAddressList;
        }
        public CustomerVM ReadCustomerEmail(string email)
        {
            CustomerVM _customer = new CustomerVM();
            var Email = email.Trim();

            using (var conn = GetOpenConnection())
            {
                _customer = conn.Query<CustomerVM>(" SELECT * FROM CUSTOMER WHERE Email = @email", new { Email }).FirstOrDefault();
            }
            return _customer;
        }
        public void CreateCustomer(Customer customer)
        {
            using (var context = GetOpenConnection())
            {
                context.Execute("" +
                     " INSERT INTO Customer" +
                     " (CustomerGuid, Username,Email, Active, Deleted, CreatedOnUtc, UpdatedOnUtc, LastActivityDateUtc) VALUES " +
                     " (@CustomerGuid, @Username,@Email, @Active, @Deleted, @CreatedOnUtc, @UpdatedOnUtc, @LastActivityDateUtc); ",
                     new
                     {
                         customer.CustomerGuid,
                         customer.Username,
                         customer.Email,
                         customer.Active,
                         customer.Deleted,
                         customer.CreatedOnUtc,
                         customer.UpdatedOnUtc,
                         customer.LastActivityDateUtc
                     });
            }
        }
        public void UpdateForgotPassword(string email)
        {
            using (var context = GetOpenConnection())
            {
                context.Execute("" +
                     " INSERT INTO ValidateViaEmail" +
                     " (CustomerGuid, Email, IsExpired, CreatedOnUtc, UpdatedOnUtc) " +
                     " VALUES (@CustomerGuid, @Email, @IsExpired, @CreatedOnUtc, @UpdatedOnUtc); ",
                     new
                     {
                         CustomerGuid = Guid.NewGuid(),
                         Email = email,
                         IsExpired = false,
                         CreatedOnUtc = DateTime.Now,
                         UpdatedOnUtc = DateTime.Now
                     });
            }
        }
        public ValidateViaEmailVM ReadForgotPasswordDetails(string email)
        {
            ValidateViaEmailVM _obj = new ValidateViaEmailVM();
            using (var conn = GetOpenConnection())
            {
                _obj = conn.Query<ValidateViaEmailVM>(" SELECT * FROM ValidateViaEmail WHERE Email =  @email ", new { email = email }).LastOrDefault();
            }
            return _obj;
        }
        public ValidateViaEmailVM ReadForgotPasswordDetails(int Id)
        {
            ValidateViaEmailVM _obj = new ValidateViaEmailVM();
            using (var conn = GetOpenConnection())
            {
                _obj = conn.Query<ValidateViaEmailVM>(" SELECT * FROM ValidateViaEmail WHERE Id =  @Id ", new { Id = Id }).LastOrDefault();
            }
            return _obj;
        }
        public ValidateViaEmailVM ReadEmailActivationDetails(string guid)
        {
            ValidateViaEmailVM _obj = new ValidateViaEmailVM();
            using (var conn = GetOpenConnection())
            {
                _obj = conn.Query<ValidateViaEmailVM>(" SELECT * FROM ValidateViaEmail WHERE CustomerGuid =  @guid ", new { guid = guid }).FirstOrDefault();
            }
            return _obj;
        }
        public void UpdateCustomerAccountStatus(int id)
        {
            using (var context = GetOpenConnection())
            {
                context.Execute(" UPDATE Customer SET Active =  @Active WHERE Id = @id; ",
                    new
                    {
                        Active = true,
                        id
                    });

            }
        }
        public void UpdateForgotPasswordDetails(int id)
        {
            using (var context = GetOpenConnection())
            {

                context.Execute(" UPDATE ValidateViaEmail SET IsExpired =  @IsExpired, UpdatedOnUtc = @UpdatedOnUtc WHERE Id = @id; ",
                    new
                    {
                        IsExpired = true,
                        UpdatedOnUtc = DateTime.Now,
                        id
                    });
            }
        }
        public IEnumerable<CustomerAddressVM> GetCustomerAllAddresses(int CustomerId)
        {
            IEnumerable<CustomerAddressVM> _CustomerAddressList = null;

            using (var conn = GetOpenConnection())
            {
                _CustomerAddressList = conn.Query<CustomerAddressVM>(" SELECT C.Id AS Customer_Id ,A.Id AS Address_Id, CONCAT(A.FirstName,' ',A.LastName) AS FullName," +
                        "A.PhoneNumber, A.Address1, A.City, A.State, A.ZipPostalCode FROM Customer C JOIN CustomerAddresses CA ON " +
                        "C.Id = CA.Customer_Id JOIN Address A ON " +
                        "CA.Address_Id = A.Id WHERE C.Id = @CustomerId AND C.Deleted = 0 ",
                new
                {
                    CustomerId
                });
            }
            return _CustomerAddressList;
        }
        public CustomerAddressVM GetCustomerAddressById(int addressId)
        {
            CustomerAddressVM _customerAddress = new CustomerAddressVM();

            using (var conn = GetOpenConnection())
            {
                _customerAddress = conn.Query<CustomerAddressVM>(" SELECT * FROM Address WHERE Id = @addressId", new { addressId }).FirstOrDefault();
            }
            return _customerAddress;
        }
        public bool SaveOrUpdateCustomerAddress(int CustomerId, Address address)
        {
            bool IsAddressSaved = false;
            int newAddressId = 0;
            using (var context = GetOpenConnection())
            {
                address.UpdatedOnUtc = DateTime.Now;
                address.CountryId = 1;  //ByDefault Saving USA Country

                //Update Address
                if (address.Id > 0)
                {
                    context.Execute("" +
                         " UPDATE Address SET FirstName = @FirstName, LastName =  @LastName, City = @City, State = @State, Company = @Company, " +
                                " Address1 = @Address1, ZipPostalCode = @ZipPostalCode,PhoneNumber=@PhoneNumber, Notes = @Notes, Deleted = @Deleted," +
                                " UpdatedOnUtc=@UpdatedOnUtc, Landmark=@Landmark, AddressType=@AddressType, IsDefaultAddress=@IsDefaultAddress, CountryId=@CountryId WHERE Id = @Id;",
                                new
                                {
                                    address.FirstName,
                                    address.LastName,
                                    address.City,
                                    address.State,
                                    address.Company,
                                    address.Address1,
                                    address.ZipPostalCode,
                                    address.PhoneNumber,
                                    address.Notes,
                                    address.Deleted,
                                    address.UpdatedOnUtc,
                                    address.Landmark,
                                    address.AddressType,
                                    address.IsDefaultAddress,
                                    address.CountryId,
                                    address.Id
                                });
                }
                //Save Address
                else
                {
                    var CustomerAddressList = GetCustomerAllAddresses(CustomerId).ToList();

                    //Update IsDefaultAddress false for every record in Address Table
                    foreach (var item in CustomerAddressList)
                    {
                        var AddressId = item.Id;
                        bool IsDefaultAddress = false;
                        context.Execute("" +
                         " UPDATE Address SET IsDefaultAddress=@IsDefaultAddress WHERE Id = @AddressId;",
                                new
                                {
                                    AddressId = item.Id,
                                    IsDefaultAddress = false
                                });
                    }

                    address.CreatedOnUtc = DateTime.Now;
                    address.IsDefaultAddress = true;


                    newAddressId = context.Query<int>("" +
                                " INSERT INTO Address (FirstName, LastName, City, State, Address1, ZipPostalCode,PhoneNumber, Notes, CreatedOnUtc,UpdatedOnUtc, Deleted, Landmark, AddressType, IsDefaultAddress, CountryId  )" +
                                " Output Inserted.Id VALUES(@FirstName, @LastName, @City, @State, @Address1, @ZipPostalCode,@PhoneNumber, @Notes, @CreatedOnUtc," +
                                " @UpdatedOnUtc, @Deleted, @Landmark, @AddressType, @IsDefaultAddress, @CountryId);",
                                    new
                                    {
                                        address.FirstName,
                                        address.LastName,
                                        address.City,
                                        address.State,
                                        address.Address1,
                                        address.ZipPostalCode,
                                        address.PhoneNumber,
                                        address.Notes,
                                        address.CreatedOnUtc,
                                        address.UpdatedOnUtc,
                                        address.Deleted,
                                        address.Landmark,
                                        address.AddressType,
                                        address.IsDefaultAddress,
                                        address.CountryId,
                                        address.Id
                                    }).Single();

                    context.Execute(" INSERT INTO CustomerAddresses(Customer_Id, Address_Id) VALUES (@Customer_Id, @newAddressId)",
                        new
                        {
                            Customer_Id = CustomerId,
                            newAddressId
                        });
                }

                IsAddressSaved = true;
            }

            return IsAddressSaved;
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
        public ShoppingCartItemsVM GetShoppingCartItemById(int Id)
        {
            ShoppingCartItemsVM _shoppingCartItemsVM = new ShoppingCartItemsVM();
            using (var conn = GetOpenConnection())
            {
                _shoppingCartItemsVM = conn.Query<ShoppingCartItemsVM>("SELECT * FROM ShoppingCartItem WHERE Id=" + Id).FirstOrDefault();
            }
            return _shoppingCartItemsVM;
        }
        public List<ShoppingCartItemsVM> GetShoppingCartItemsByCartId(int CartId)
        {
            List<ShoppingCartItemsVM> _cartItemsList = new List<ShoppingCartItemsVM>();
            using (var conn = GetOpenConnection())
            {
                _cartItemsList = conn.Query<ShoppingCartItemsVM>("SELECT * FROM ShoppingCartItem WHERE CartId=" + CartId).ToList();
            }

            return _cartItemsList;
        }
        public CustomerCartVM GetCustomerShoppingCartDetails(int CustomerId)
        {
            CustomerCartVM _customerCartVM = null;
            using (var conn = GetOpenConnection())
            {
                //Get CustomerCart by CustomerId
                var CartId = conn.Query<int>("SELECT CartId from CustomerCart WHERE CustomerId=@CustomerId",
                    new
                    {
                        CustomerId = CustomerId
                    }).FirstOrDefault();

                if (CartId > 0)
                {
                    //Get ShoppingCartItems By CartId
                    List<ShoppingCartItemsVM> _cartItemsList = GetShoppingCartItemsByCartId(CartId);
                    if (_cartItemsList.Count > 0)
                    {
                        decimal ProductsPrice = 0;

                        foreach (var item in _cartItemsList)
                        {
                            var _prod = GetProductById(item.ProductId);
                            item.ProductPrice = _prod.Price;
                            ProductsPrice = ProductsPrice + (_prod.Price * item.Quantity);
                        }

                        _customerCartVM = new CustomerCartVM();

                        _customerCartVM.CartId = CartId;
                        _customerCartVM.CustomerId = CustomerId;
                        _customerCartVM.CartProducts = _cartItemsList;
                        _customerCartVM.DeliveryCharge = (ProductsPrice < 500) ? 50 : 0;
                        _customerCartVM.ProductsPrice = ProductsPrice;
                        _customerCartVM.TotalCartAmt = (_customerCartVM.DeliveryCharge + _customerCartVM.ProductsPrice);
                    }
                }
            }

            return _customerCartVM;
        }
        public Tuple<bool, string> AddToCart(int CustomerId, int ProductId)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");

            using (var conn = GetOpenConnection())
            {
                //Check whether Customer Exists cart or not
                var CustomerCartObj = conn.Query<CustomerCartVM>("SELECT * FROM CustomerCart WHERE CustomerId=" + CustomerId).FirstOrDefault();

                //Add new Record for that Customer in CustomerCart and ShoppingCartItem Tables
                if (CustomerCartObj == null)
                {
                    int newcartId = 0;

                    //Add Record in Customer Cart Table
                    newcartId = conn.Query<int>(" INSERT INTO CustomerCart(CustomerId) Output Inserted.CartId VALUES (@CustomerId)",
                       new
                       {
                           CustomerId = CustomerId
                       }).SingleOrDefault();

                    //Add Cart Items in ShoppingCartItem Table
                    conn.Execute
                                   (" INSERT INTO ShoppingCartItem ( ProductId, CustomerId, Quantity, CreatedOnUtc, UpdatedOnUtc, CartItemGuid, CartId) " +
                                           " VALUES ( @ProductId, @CustomerId, @Quantity, @CreatedOnUtc, @UpdatedOnUtc, @CartItemGuid, @CartId)",
                                       new
                                       {
                                           ProductId = ProductId,
                                           CustomerId = CustomerId,
                                           Quantity = 1,
                                           CreatedOnUtc = DateTime.UtcNow,
                                           UpdatedOnUtc = DateTime.UtcNow,
                                           CartItemGuid = Guid.NewGuid(),
                                           CartId = newcartId
                                       });
                }

                //Update Product Quantity in ShoppingCartItem Tables
                else
                {
                    var ShoppingCartList = conn.Query<ShoppingCartItemsVM>("SELECT * FROM ShoppingCartItem WHERE CartId=" + CustomerCartObj.CartId).ToList();

                    //Get the Selected Product from ShoppingCartItem
                    var shoppingProductItem = ShoppingCartList.Where(m => m.ProductId == ProductId).FirstOrDefault();

                    //If Product Does'nt exists in ShoppingCartItem Table then add record 
                    if (shoppingProductItem == null)
                    {
                        conn.Execute
                                   (" INSERT INTO ShoppingCartItem (CustomerId, CartId, ProductId, Quantity, CreatedOnUtc, UpdatedOnUtc, CartItemGuid) " +
                                           " VALUES (@CustomerId, @CartId, @ProductId, @Quantity, @CreatedOnUtc, @UpdatedOnUtc, @CartItemGuid)",
                                       new
                                       {
                                           CustomerId = CustomerId,
                                           CartId = CustomerCartObj.CartId,
                                           ProductId = ProductId,
                                           Quantity = 1,
                                           CreatedOnUtc = DateTime.UtcNow,
                                           UpdatedOnUtc = DateTime.UtcNow,
                                           CartItemGuid = Guid.NewGuid(),
                                       });
                    }
                    //If Product exists in ShoppingCartItem Table then update the Product Quantity
                    else
                    {
                        conn.Execute("" +
                                 " UPDATE ShoppingCartItem SET Quantity = @Quantity, UpdatedOnUtc =  @UpdatedOnUtc " +
                                 " WHERE Id = @Id;",
                                 new
                                 {
                                     Id = shoppingProductItem.Id,
                                     Quantity = shoppingProductItem.Quantity + 1,
                                     UpdatedOnUtc = DateTime.UtcNow,
                                 });
                    }
                }

                tuple = new Tuple<bool, string>(true, "Item Added Successfully");
            }

            return tuple;
        }
        public Tuple<bool, string> UpdateCart(int CartId, int ProductId, int Quantity)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");

            using (var conn = GetOpenConnection())
            {
                //Remove the Product from ShoppingCartItem
                if (Quantity == 0)
                {
                    var _removeProductResult = DeleteProductFromCart(CartId, ProductId);
                    tuple = (_removeProductResult.Item1 == true) ? new Tuple<bool, string>(true, "Item Removed Successfully") : new Tuple<bool, string>(false, "");
                }
                else
                {
                    conn.Execute("" +
                                 " UPDATE ShoppingCartItem SET Quantity = @Quantity, UpdatedOnUtc =  @UpdatedOnUtc " +
                                 " WHERE ProductId = @ProductId and CartId=@CartId;",
                                 new
                                 {
                                     CartId = CartId,
                                     ProductId = ProductId,
                                     Quantity = Quantity,
                                     UpdatedOnUtc = DateTime.UtcNow,
                                 });
                    tuple = new Tuple<bool, string>(true, "Cart Updated Successfully");
                }
            }

            return tuple;
        }
        public Tuple<bool, string> DeleteProductFromCart(int CartId, int ProductId)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");

            using (var conn = GetOpenConnection())
            {
                //Get Product from ShoppingCartItem By CartId
                var _cartItemsList = GetShoppingCartItemsByCartId(CartId);

                //Delete Record from ShoppingCartItem
                if (_cartItemsList.Count > 0)
                {
                    var cartItemObj = _cartItemsList.Where(m => m.ProductId == ProductId).FirstOrDefault();
                    if (cartItemObj != null)
                    {
                        conn.Execute("" +
                                      " DELETE from ShoppingCartItem " +
                                      " WHERE Id = @Id;",
                                      new
                                      {
                                          Id = cartItemObj.Id,
                                      });

                        //Delete ShoppingCartItem and Delete Record from CustomerCart Table
                        if (_cartItemsList.Count == 1)
                        {
                            conn.Execute("" +
                                      " DELETE from CustomerCart " +
                                      " WHERE CartId = @Id;",
                                      new
                                      {
                                          Id = CartId,
                                      });
                        }
                        return tuple = new Tuple<bool, string>(true, "Item Deleted Successfully");
                    }
                    return tuple = new Tuple<bool, string>(false, "Product Does'nt Exists in your Cart");
                }
            }

            return tuple;
        }
    }
}

