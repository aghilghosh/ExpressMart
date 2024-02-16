using SmartCart.Abstraction;
using System.Collections.Generic;

namespace SmartCart.Offers
{
    public class OfferBuyOnItemCount : AbstractDiscount
    {
        private readonly int _itemBuy;

        private readonly int _itemFree;

        private IEnumerable<IProduct>? _applicableItems = null;
        public override bool CanClubbedWithOtherDiscounts { get; set; }

        /// <summary>
        /// Defines the discount.
        /// </summary>
        /// <param name="itemBuy">Number of items to buy to eligibe for this offer.</param>
        /// <param name="itemFree">Number of items free.</param>
        /// <param name="canClubbedWithOtherDiscounts">Can clubbed with other discounts/offers.</param>
        public OfferBuyOnItemCount(int itemBuy, int itemFree, bool canClubbedWithOtherDiscounts = false)
        {
            _itemBuy = itemBuy;
            _itemFree = itemFree;
            CanClubbedWithOtherDiscounts = canClubbedWithOtherDiscounts;
        }

        /// <summary>
        /// Defines the discount.
        /// </summary>
        /// <param name="itemBuy">Number of items to buy to eligibe for this offer.</param>
        /// <param name="itemFree">Number of items free.</param>
        /// <param name="applicableItems">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        /// <param name="canClubbedWithOtherDiscounts">Can clubbed with other discounts/offers.</param>
        public OfferBuyOnItemCount(int itemBuy, int itemFree, IEnumerable<IProduct> applicableItems, bool canClubbedWithOtherDiscounts = false)
        {
            _itemBuy = itemBuy;
            _itemFree = itemFree;
            _applicableItems = applicableItems;
            CanClubbedWithOtherDiscounts = canClubbedWithOtherDiscounts;
        }

        /// <summary>
        /// Process the discount.
        /// </summary>
        /// <param name="orderSummary">Order summary.</param>
        /// <param name="applicableItems">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        public override void ProcessDiscount(OrderSummary orderSummary, IEnumerable<IProduct> applicableItems)
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

            _applicableItems = null;
        }

        private void ApplyDiscount(OrderItem eligibleItem)
        {
            if (eligibleItem.Quantity > _itemBuy)
            {
                var extra = eligibleItem.Quantity % (_itemBuy + _itemFree);
                var price = 0.0m;
                if (extra == 0)
                {
                    price = (eligibleItem.Quantity / (_itemBuy + _itemFree)) * _itemBuy * eligibleItem.UnitPrice;
                }
                else
                {
                    price = (((eligibleItem.Quantity / (_itemBuy + _itemFree)) * _itemBuy) + extra) * eligibleItem.UnitPrice;
                }

                eligibleItem.DiscountCategory = true;
                eligibleItem.GrossPrice = price;
                eligibleItem.DiscountApplied += (eligibleItem.UnitPrice * eligibleItem.Quantity) - price;
                eligibleItem.AppliedDiscounts = eligibleItem.AppliedDiscounts.Replace("Not applicable", "");
                eligibleItem.AppliedDiscounts += $"Buy {_itemBuy} get {_itemFree} applied ";
            } else if (eligibleItem.Quantity == _itemBuy)
            {
                eligibleItem.AppliedDiscounts = eligibleItem.AppliedDiscounts.Replace("Not applicable", "");
                eligibleItem.AppliedDiscounts += $"Buy {_itemBuy} get {_itemFree} applied ";
            }
        }
    }
}
