using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class CartVM
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string OrderType { get; set; }
        public bool IsDeleteItem { get; set; }
        public Nullable<DateTime> CreatedOnUtc { get; set; }
        public Nullable<DateTime> UpdatedOnUtc { get; set; }
    }

    public class CustomerCartVM
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public List<ShoppingCartItemsVM> CartProducts { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal ProductsPrice { get; set; }
        public decimal TotalCartAmt { get; set; }
    }

    public class ShoppingCartItemsVM
    {
        public int Id { get; set; }
        public System.Guid CartItemGuid { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public Nullable<DateTime> CreatedOnUtc { get; set; }
        public Nullable<DateTime> UpdatedOnUtc { get; set; }
    }
}
