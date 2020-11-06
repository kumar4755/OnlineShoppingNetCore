using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.FrontEnd.Repository;
using OnlineShopping.Models;

namespace OnlineShopping.FrontEnd.Api.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository = null;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }


        [Route("AddOrUpdateToCart")]
        [HttpPost]
        public IActionResult AddOrUpdateToCart(CartVM cart)
        {
            try
            {
                var IsAdded = _customerRepository.AddOrUpdateToCart(cart);
                return Ok(IsAdded);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("SaveOrUpdateCustomerAddress")]
        [HttpPost]
        public IActionResult SaveOrUpdateCustomerAddress(int CustomerId, Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            bool IsAddressSaved = _customerRepository.SaveOrUpdateCustomerAddress(CustomerId, address);
            return Ok(IsAddressSaved);
        }

        [Route("GetCustomerAllAddresses")]
        [HttpPost]
        public IActionResult GetCustomerAllAddresses(int CustomerId)
        {
            var _customerAddresses = _customerRepository.GetCustomerAllAddresses(CustomerId);
            return Ok(_customerAddresses);
        }

        [Route("GetCustomerAddressById")]
        [HttpGet]
        public IActionResult GetCustomerAddressById(int addressId)
        {
            var _customerAddresses = _customerRepository.GetCustomerAddressById(addressId);
            return Ok(_customerAddresses);
        }

        [Route("GetCustomerShoppingCartDetails")]
        [HttpGet]
        public IActionResult GetCustomerShoppingCartDetails(int CustomerId)
        {
            var _customercart = _customerRepository.GetCustomerShoppingCartDetails(CustomerId);
            return Ok(_customercart);
        }

        [Route("AddToCart")]
        [HttpPost]
        public IActionResult AddToCart(int CustomerId, int ProductId)
        {
            try
            {
                var IsAdded = _customerRepository.AddToCart(CustomerId, ProductId);
                return Ok(IsAdded);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("UpdateCart")]
        [HttpPost]
        public IActionResult UpdateCart(int CartId, int ProductId, int Quantity)
        {
            try
            {
                var IsAdded = _customerRepository.UpdateCart(CartId, ProductId, Quantity);
                return Ok(IsAdded);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("DeleteProductFromCart")]
        [HttpPost]
        public IActionResult DeleteProductFromCart(int CartId, int ProductId)
        {
            try
            {
                var IsAdded = _customerRepository.DeleteProductFromCart(CartId, ProductId);
                return Ok(IsAdded);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}