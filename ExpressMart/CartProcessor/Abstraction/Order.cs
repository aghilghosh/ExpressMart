using System.Collections.Generic;

namespace SmartCart.Abstraction
{
    public class OrderSummary
    {
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount
        {
            get
            {
                decimal total = 0;

                Items.ForEach(i => { total = total + i.GrossPrice; });
                return total;
            }
        }
        public decimal DiscountedTotal {

            get
            {
                decimal total = 0;

                Items.ForEach(i => { total = total + i.DiscountApplied; });
                return total;
            }
        }
        public decimal DiscountedSavedOnPurchase { get; set; }
    }

    public class OrderItem
    {
        public int Quantity { get; set; }

        public string ProductName { get; set; }

        public int ProductCode { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal GrossPrice { get; set; }

        public decimal DiscountApplied { get; set; }

        public bool DiscountCategory { get; set; }

        public string AppliedDiscounts { get; set; } = "Not applicable";
    }
}
