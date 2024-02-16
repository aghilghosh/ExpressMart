using SmartCart.Abstraction;
using System.Collections.Generic;

namespace SmartCart.Offers
{
    public class OfferBuyOnSpent : AbstractDiscount
    {
        private readonly decimal _spentAmount;

        private readonly decimal _discount;

        private IEnumerable<IProduct>? _applicableItems = null;

        public override bool CanClubbedWithOtherDiscounts { get; set; }

        /// <summary>
        /// Defines the discount.
        /// </summary>
        /// <param name="amount">Amount to spent to qualify for this offer.</param>
        /// <param name="percentageOff">Percentage of discount.</param>
        /// <param name="canClubbedWithOtherDiscounts">Can clubbed with other discounts/offers.</param>
        public OfferBuyOnSpent(decimal amount, decimal percentageOff, bool canClubbedWithOtherDiscounts = false)
        {
            _spentAmount = amount;
            _discount = percentageOff;
            CanClubbedWithOtherDiscounts = canClubbedWithOtherDiscounts;
        }

        /// <summary>
        /// Defines the discount.
        /// </summary>
        /// <param name="amount">Amount to spent to qualify for this offer.</param>
        /// <param name="percentageOff">Percentage of discount.</param>
        /// <param name="applicableItems">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        /// <param name="canClubbedWithOtherDiscounts">Can clubbed with other discounts/offers.</param>
        public OfferBuyOnSpent(decimal amount, decimal percentageOff, IEnumerable<IProduct> applicableItems, bool canClubbedWithOtherDiscounts = false)
        {
            _spentAmount = amount;
            _discount = percentageOff;
            _applicableItems = applicableItems;
            CanClubbedWithOtherDiscounts = canClubbedWithOtherDiscounts;
        }

        /// <summary>
        /// Process the discount.
        /// </summary>
        /// <param name="orderSummary">Order summary.</param>
        /// <param name="applicableItems">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        public override void ProcessDiscount(OrderSummary orderSummary, IEnumerable<IProduct> applicableItems = null)
        {
            if (orderSummary.TotalAmount > _spentAmount)
            {
                foreach (var eligibleItem in ApplicableItems(orderSummary, _applicableItems ?? applicableItems))
                {
                    var discountedAmount = eligibleItem.GrossPrice * _discount / 100;
                    eligibleItem.GrossPrice -= discountedAmount;
                    eligibleItem.DiscountApplied += discountedAmount;
                    eligibleItem.DiscountCategory = true;
                    eligibleItem.AppliedDiscounts = eligibleItem.AppliedDiscounts.Replace("Not applicable", "");
                    eligibleItem.AppliedDiscounts += $"{_discount}% off applied on spent of {_spentAmount} applied ";
                }
            }

            _applicableItems = null;
        }
    }
}
