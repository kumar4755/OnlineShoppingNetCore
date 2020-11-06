using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShopping.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AuthorName { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string AdminComment { get; set; }
        public int CategoryId { get; set; }
        public string PicturePath { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool AllowCustomerReviews { get; set; }
        public Nullable<double> ApprovedRatingSum { get; set; }
        public Nullable<double> NotApprovedRatingSum { get; set; }
        public Nullable<int> ApprovedTotalReviews { get; set; }
        public Nullable<int> NotApprovedTotalReviews { get; set; }
        public bool IsFreeShipping { get; set; }
        public decimal AdditionalShippingCharge { get; set; }
        public int StockQuantity { get; set; }
        public Nullable<bool> DisplayStockAvailability { get; set; }
        public Nullable<bool> DisplayStockQuantity { get; set; }
        public Nullable<int> NotifyAdminForQuantityBelow { get; set; }
        public Nullable<int> OrderMinimumQuantity { get; set; }
        public Nullable<int> OrderMaximumQuantity { get; set; }
        public Nullable<bool> AvailableForPreOrder { get; set; }
        public Nullable<System.DateTime> PreOrderAvailabilityStartDateTimeUtc { get; set; }
        public decimal Price { get; set; }
        public decimal ProductCost { get; set; }
        public Nullable<decimal> SpecialPrice { get; set; }
        public Nullable<System.DateTime> SpecialPriceStartDateTimeUtc { get; set; }
        public Nullable<System.DateTime> SpecialPriceEndDateTimeUtc { get; set; }
        public Nullable<bool> MarkAsNew { get; set; }
        public Nullable<System.DateTime> MarkAsNewStartDateTimeUtc { get; set; }
        public Nullable<System.DateTime> MarkAsNewEndDateTimeUtc { get; set; }
        public Nullable<bool> HasDiscountsApplied { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<System.DateTime> AvailableStartDateTimeUtc { get; set; }
        public Nullable<System.DateTime> AvailableEndDateTimeUtc { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
        public Nullable<bool> Buyable { get; set; }
        public Nullable<bool> Rentable { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public System.DateTime UpdatedOnUtc { get; set; }
    }
}
