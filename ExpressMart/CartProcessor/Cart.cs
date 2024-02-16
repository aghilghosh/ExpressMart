using SmartCart.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCart
{
    /// <summary>
    /// Create an cart object to add items into the shopping cart and process order summary.
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Creates a cart object.
        /// </summary>
        public Cart()
        {
            _lineItems = new List<IProduct>();
            _productsOnOffer = new List<IProduct>();
            _discountTypes = new List<AbstractDiscount>();
        }

        /// <summary>
        /// Add item into the cart.
        /// </summary>
        /// <param name="items">Items to add to cart.</param>
        /// <returns>The cart.</returns>
        public Cart WithItems(IEnumerable<IProduct> items)
        {
            _lineItems = items;
            return this;
        }

        /// <summary>
        /// Products on offer.
        /// </summary>
        /// <param name="items">Provide a list of products with product code common for applicable discounts. The offer applyies to all items in the cart if this property is null or empty.</param>
        /// <returns>The cart.</returns>
        public Cart WithOfferOnProducts(List<IProduct> productsOnOffer)
        {
            _productsOnOffer = productsOnOffer;
            return this;
        }

        /// <summary>
        /// Provide discount type inherited from AbstactDiscount class.
        /// </summary>
        /// <param name="dicountType">Duscount type.</param>
        /// <returns>The cart.</returns>
        public Cart WithDiscount(AbstractDiscount dicountType)
        {
            _discountTypes.Add(dicountType);
            return this;
        }

        /// <summary>
        /// Builds the cart.
        /// </summary>
        /// <returns>The cart.</returns>
        public Cart Build()
        {
            try
            {
                if (!_lineItems.Any()) { throw new Exception("Order items are not found."); }

                _orderSummary = BuildOrderSummary(_lineItems);

                if (_discountTypes.Any()) {

                    foreach (var discount in _discountTypes)
                    {
                        discount.ProcessDiscount(_orderSummary, _productsOnOffer);
                    }
                }                
            }
            catch (Exception)
            {
                throw;
            }

            return this;
        }

        /// <summary>
        /// Order checkout.
        /// </summary>
        /// <returns>The order summary.</returns>
        public OrderSummary CheckOut()
        {
            return _orderSummary;
        }

        /// <summary>
        /// Builds order summary from cart items before applying discount.
        /// </summary>
        /// <param name="items">Cart items.</param>
        /// <returns>Order summary.</returns>
        private OrderSummary BuildOrderSummary(IEnumerable<IProduct> items)
        {
            var orderSummary = new OrderSummary() { Items = new List<OrderItem>() };

            if (items != null && items.Any())
            {
                var groupedItems = items.GroupBy(i => i.ProductCode);

                groupedItems.ToList().ForEach(i =>
                {
                    var itemCount = i.Count();
                    var itemSummary = i.First();

                    orderSummary.Items.Add(new OrderItem() { 

                        ProductCode = i.Key, Quantity = i.Count(), 
                        UnitPrice = itemSummary.Price, 
                        GrossPrice = itemCount * itemSummary.Price, 
                        ProductName = itemSummary.ProductName, 
                        DiscountApplied = 0
                    });
                });
            }

            return orderSummary;
        }

        /// <summary>
        /// The order summary.
        /// </summary>
        public OrderSummary _orderSummary { get; set; }

        private List<AbstractDiscount> _discountTypes { get; set; }

        private IEnumerable<IProduct> _lineItems { get; set; }

        private IEnumerable<IProduct> _productsOnOffer { get; set; }
    }
}
