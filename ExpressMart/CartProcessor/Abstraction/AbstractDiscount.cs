using System.Collections.Generic;
using System.Linq;

namespace SmartCart.Abstraction
{
    /// <summary>
    /// Implement AbstractDiscount to define new discount types.
    /// </summary>
    public abstract class AbstractDiscount
    {
        public AbstractDiscount() { }

        public abstract bool CanClubbedWithOtherDiscounts { get; set; }

        /// <summary>
        /// Finds the items which are available for discount.
        /// </summary>
        /// <param name="orderSummary">Order summary.</param>
        /// <param name="applicableItems">Items available on discount.</param>
        /// <returns>The order summary.</returns>
        public IEnumerable<OrderItem> ApplicableItems(OrderSummary orderSummary, IEnumerable<IProduct> applicableItems)
        {
            return applicableItems != null && applicableItems.Any() ? orderSummary.Items.Where(i => applicableItems.Any(a => a.ProductCode == i.ProductCode)) : orderSummary.Items;
        }

        /// <summary>
        /// Executes the discount on order summary.
        /// </summary>
        /// <param name="orderSummary">Order summary.</param>
        /// <param name="applicableItems">Items available on discount.</param>
        public abstract void ProcessDiscount(OrderSummary orderSummary, IEnumerable<IProduct> applicableItems);
    }
}
