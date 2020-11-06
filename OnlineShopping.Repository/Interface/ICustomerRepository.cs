using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.FrontEnd.Repository
{
    public interface ICustomerRepository
    {
        Tuple<bool, string> AddOrUpdateToCart(CartVM cart);
        int ReadCustomerId(string username);
        CustomerDetails ReadCustomerDetails(int Id);
        CustomerProfileVM ReadCustomerProfile(int customerId);
        IEnumerable<CustomerAddressVM> ReadCustomerAddress(int Id);
        CustomerVM ReadCustomerEmail(string email);
        void CreateCustomer(Customer customer);
        void UpdateForgotPassword(string email);
        ValidateViaEmailVM ReadForgotPasswordDetails(string email);
        ValidateViaEmailVM ReadForgotPasswordDetails(int Id);
        ValidateViaEmailVM ReadEmailActivationDetails(string guid);
        void UpdateCustomerAccountStatus(int id);
        void UpdateForgotPasswordDetails(int id);
        IEnumerable<CustomerAddressVM> GetCustomerAllAddresses(int CustomerId);
        CustomerAddressVM GetCustomerAddressById(int addressId);
        bool SaveOrUpdateCustomerAddress(int CustomerId, Address address);
        CustomerCartVM GetCustomerShoppingCartDetails(int CustomerId);
        Tuple<bool, string> AddToCart(int CustomerId, int ProductId);
        Tuple<bool, string> UpdateCart(int CartId, int ProductId, int Quantity);
        Tuple<bool, string> DeleteProductFromCart(int CartId, int ProductId);
    }
}
