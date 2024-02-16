using SmartCart.Abstraction;
using System.Collections.Generic;

namespace SmartCart.Offers
{
    public class OfferBuyDicountOnProduct : AbstractDiscount
    {
        private decimal _percentageDicount;

        private IEnumerable<IProduct>? _applicableItems = null;

        /// <summary>
        /// Defines the discount.
        /// </summary>
        /// <param name="discount">Discount percentage.</param>
        /// <param name="canClubbedWithOtherDiscounts">Can clubbed with other discounts/offers.</param>
        public OfferBuyDicountOnProduct(decimal discount, bool canClubbedWithOtherDiscounts = false)
        {
            _percentageDicount = discount;
            CanClubbedWithOtherDiscounts = canClubbedWithOtherDiscounts;
        }

        /// <summary>
        /// Defines the discount.
        /// </summary>
        /// <param name="discount">Discount percentage.</param>
        /// <param name="applicableItems">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        /// <param name="canClubbedWithOtherDiscounts">Can clubbed with other discounts/offers.</param>
        public OfferBuyDicountOnProduct(decimal discount, IEnumerable<IProduct> applicableItems, bool canClubbedWithOtherDiscounts = false)
        {
            _percentageDicount = discount;
            _applicableItems = applicableItems;
            CanClubbedWithOtherDiscounts = canClubbedWithOtherDiscounts;
        }

        public override bool CanClubbedWithOtherDiscounts { get; set; }

        /// <summary>
        /// Process the discount.
        /// </summary>
        /// <param name="orderSummary">Order summary.</param>
        /// <param name="applicableItems">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        public override void ProcessDiscount(OrderSummary orderSummary, IEnumerable<IProduct> applicableItems)
        {
            if (_percentageDicount > 0)
            {
                foreach (var eligibleItem in ApplicableItems(orderSummary, _applicableItems ?? applicableItems))
                {
                    if (CanClubbedWithOtherDiscounts)
                    {
                        ApplyDiscount(eligibleItem);
                    }
                    else if (!eligibleItem.DiscountCategory)
                    {
                        ApplyDiscount(eligibleItem);
                    }
                }
            }

            _applicableItems = null;
        }

        private void ApplyDiscount(OrderItem eligibleItem)
        {
            var discountedAmount = eligibleItem.GrossPrice * _percentageDicount / 100;

            eligibleItem.GrossPrice -= discountedAmount;
            eligibleItem.DiscountApplied += discountedAmount;
            eligibleItem.DiscountCategory = true;
            eligibleItem.AppliedDiscounts = eligibleItem.AppliedDiscounts.Replace("Not applicable", "");
            eligibleItem.AppliedDiscounts += $"{_percentageDicount}% applied ";
        }
    }
}
