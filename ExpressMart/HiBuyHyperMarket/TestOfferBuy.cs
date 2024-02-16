using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartCart;
using SmartCart.Abstraction;
using SmartCart.Offers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HiBuyHyperMarket
{
    [TestClass]
    public class TestOfferBuy
    {
        private IEnumerable<IProduct> stockItems;

        [TestInitialize]
        public void TestSetup()
        {
            // lookup items.
            stockItems = new List<IProduct>()
            {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Getorade 1l", ProductCode = 0025004, Price = 48.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Slice 1l", ProductCode = 0025006, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Good day 100gm", ProductCode = 0035001, Price = 25.0m },
                new HiBuyProducts (){ ProductName = "Bouboun 100gm", ProductCode = 0035002, Price = 22.0m },
                new HiBuyProducts (){ ProductName = "Boost 500gm", ProductCode = 0035003, Price = 350.0m },
                new HiBuyProducts (){ ProductName = "Milo 250gm", ProductCode = 0035004, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Burberry 80gm", ProductCode = 0455001, Price = 150.0m },
                new HiBuyProducts (){ ProductName = "Cadbury Dairy milk 100gm", ProductCode = 0455002, Price = 225.0m },
                new HiBuyProducts (){ ProductName = "Galaxy Chocolate 120gm", ProductCode = 0455003, Price = 365.0m },
                new HiBuyProducts (){ ProductName = "Hersheley chocodip 150gm", ProductCode = 0455004, Price = 400.0m },
                new HiBuyProducts (){ ProductName = "Kitkat 80gm", ProductCode = 0455005, Price = 150.0m },
                new HiBuyProducts (){ ProductName = "Cadbury 5 stars", ProductCode = 0455006, Price = 95.0m },
                new HiBuyProducts (){ ProductName = "Twix 90gm", ProductCode = 0455007, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Oreo 150gm", ProductCode = 0455008, Price = 50.0m },
                new HiBuyProducts (){ ProductName = "Cremea 80gm", ProductCode = 0455009, Price = 325.0m },
                new HiBuyProducts (){ ProductName = "Sunfeast 180gm", ProductCode = 0455010, Price = 70.0m }
            };
        }

        [TestMethod]
        public void OrderSummaryWithNoDiscountApplied()
        {
            // cart items.
            var cartItems = new List<IProduct>() {

                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
            };

            var orderSummary = new Cart().WithItems(cartItems).Build().CheckOut();

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(316.0m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(0, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
        }

        [TestMethod]
        public void OrderSummaryWithFlatDiscountAppliedOnSelectedProducts()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m }
            };

            var flatDiscountedItems = new List<IProduct>()
            {
                new HiBuyProducts (){ ProductCode = 0025001 },
                new HiBuyProducts (){ ProductCode = 0025004 },
            };

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyDicountOnProduct(30, flatDiscountedItems))
                .Build()
                .CheckOut();

            this.WriteOrderSummary(orderSummary);

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(253.0m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(54, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(54, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual("30% applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(25001, orderSummary.Items[0].ProductCode, "Order discount not applied to eligible product, product code does not match");
            Assert.AreEqual(0, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
        }

        [TestMethod]
        public void OrderSummaryWithFlatDiscountAppliedOnAllProducts()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m }
            };

            var flatDiscountedItems = new List<IProduct>();

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyDicountOnProduct(30, flatDiscountedItems))
                .Build()
                .CheckOut();

            this.WriteOrderSummary(orderSummary);

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(214.9m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(92.1m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(54, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual("30% applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(14.1m, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% applied ", orderSummary.Items[1].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(24, orderSummary.Items[2].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% applied ", orderSummary.Items[2].AppliedDiscounts, "Order summary item wormg discount applied");
        }

        [TestMethod]
        public void OrderSummaryWithDiscountOnSpentAppliedOnAllItems()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m }
            };

            var spendOnTarget = new List<IProduct>();

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyOnSpent(300, 30, spendOnTarget))
                .Build()
                .CheckOut();

            this.WriteOrderSummary(orderSummary);

            Assert.IsNotNull(orderSummary, "Order checkout successded");
            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(214.9m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(92.1m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(54, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual("30% off applied on spent of 300 applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(14.1m, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% off applied on spent of 300 applied ", orderSummary.Items[1].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(24, orderSummary.Items[2].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% off applied on spent of 300 applied ", orderSummary.Items[2].AppliedDiscounts, "Order summary item wormg discount applied");
        }

        [TestMethod]
        public void OrderSummaryWithDiscountOnSpentAppliedOnSelectedItems()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m }
            };

            var spendOnTarget = new List<IProduct>()
            {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m }
            };

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyOnSpent(150, 30, spendOnTarget))
                .Build()
                .CheckOut();

            Assert.IsNotNull(orderSummary, "Order checkout successded");
            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(238.9m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(68.1m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(54, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual("30% off applied on spent of 150 applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(14.1m, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% off applied on spent of 150 applied ", orderSummary.Items[1].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(0, orderSummary.Items[2].DiscountApplied, "Order summary discount applied to wrong product");
        }

        [TestMethod]
        public void OrderSummaryWithDiscountOnSpentNotApplied()
        {
            // amount spent is less than the discount amount
            var cartItems = new List<IProduct>() {

                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
            };

            var spendOnTarget = new List<IProduct>();

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyOnSpent(1000, 30, spendOnTarget))
                .Build()
                .CheckOut();

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(316.0m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(0, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
        }

        [TestMethod]
        public void OrderSummaryWithBuyAndGetFreeApplied()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m }
            };

            var flatDiscountedItems = new List<IProduct>();

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyDicountOnProduct(30, flatDiscountedItems))
                .Build()
                .CheckOut();

            this.WriteOrderSummary(orderSummary);

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(3, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(214.9m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(92.1m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(54, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual("30% applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(14.1m, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% applied ", orderSummary.Items[1].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(24, orderSummary.Items[2].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual("30% applied ", orderSummary.Items[2].AppliedDiscounts, "Order summary item wormg discount applied");
        }

        [TestMethod]
        public void OrderSummaryWithBuyAndGetOfferOnSelectedItems()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
            };

            var buygetItemfree = new List<IProduct>()
            {
                 new HiBuyProducts (){ ProductCode = 0025001 },
                 new HiBuyProducts (){ ProductCode = 0025003 },
            };

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyOnItemCount(2, 1, buygetItemfree))
                .Build()
                .CheckOut();

            this.WriteOrderSummary(orderSummary);

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(4, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(532.0m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(90.0m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(270.0m, orderSummary.Items[0].GrossPrice, "Order summary eligible item amount does not match");
            Assert.AreEqual(90.0m, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(8, orderSummary.Items[0].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual("Buy 2 get 1 applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(0, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual(2, orderSummary.Items[2].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual("Buy 2 get 1 applied ", orderSummary.Items[2].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(0, orderSummary.Items[2].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(3, orderSummary.Items[3].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual(0, orderSummary.Items[3].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(135.0m, orderSummary.Items[3].GrossPrice, "Order summary eligible item amount does not match");
        }

        [TestMethod]
        public void OrderSummaryWithBuyAndGetOfferOnAllItems()
        {
            // cart items.
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
            };

            var buygetItemfree = new List<IProduct>();

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyOnItemCount(2, 1, buygetItemfree))
                .Build()
                .CheckOut();
            this.WriteOrderSummary(orderSummary);
            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(4, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(487.0m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(135.0m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");
            Assert.AreEqual(270.0m, orderSummary.Items[0].GrossPrice, "Order summary eligible item amount does not match");
            Assert.AreEqual(90.0m, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(8, orderSummary.Items[0].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual("Buy 2 get 1 applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(0, orderSummary.Items[1].DiscountApplied, "Order summary discount applied to wrong product");
            Assert.AreEqual(2, orderSummary.Items[2].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual(0, orderSummary.Items[2].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(3, orderSummary.Items[3].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual(45.0m, orderSummary.Items[3].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(90.0m, orderSummary.Items[3].GrossPrice, "Order summary eligible item amount does not match");
            Assert.AreEqual("Buy 2 get 1 applied ", orderSummary.Items[3].AppliedDiscounts, "Order summary item wormg discount applied");
        }

        [TestMethod]
        public void OrderSummaryWithAllDiscountApplied()
        {
            var cartItems = new List<IProduct>() {
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Pepsi 1l", ProductCode = 0025001, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Cocacola 1l", ProductCode = 0025002, Price = 47.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mirinda 1l", ProductCode = 0025003, Price = 40.0m },
                new HiBuyProducts (){ ProductName = "Mountain dew 1l", ProductCode = 0025005, Price = 45.0m },
                new HiBuyProducts (){ ProductName = "Kitkat 80gm", ProductCode = 0455005, Price = 150.0m },
                new HiBuyProducts (){ ProductName = "Cadbury 5 stars", ProductCode = 0455006, Price = 95.0m },
                new HiBuyProducts (){ ProductName = "Twix 90gm", ProductCode = 0455007, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Twix 90gm", ProductCode = 0455007, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Twix 90gm", ProductCode = 0455007, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Twix 90gm", ProductCode = 0455007, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Twix 90gm", ProductCode = 0455007, Price = 275.0m },
                new HiBuyProducts (){ ProductName = "Oreo 150gm", ProductCode = 0455008, Price = 50.0m },
                new HiBuyProducts (){ ProductName = "Cremea 80gm", ProductCode = 0455009, Price = 325.0m },
                new HiBuyProducts (){ ProductName = "Sunfeast 180gm", ProductCode = 0455010, Price = 70.0m }
            };

            // flat 5% discount on beverages.
            var flatDiscount = new List<IProduct>() {
                new HiBuyProducts (){ ProductCode = 0025001 },
                new HiBuyProducts (){ ProductCode = 0025002 },
                new HiBuyProducts (){ ProductCode = 0025003 }
            };

            // buy 3 Twix, get 1 free.
            // buy 1 Oreo, get one free.
            var buygetItemfree = new List<IProduct>() {
                new HiBuyProducts (){ ProductCode = 0455007 }
            };

            var buyOnegetItemfree = new List<IProduct>() {
                new HiBuyProducts (){ ProductCode = 0455008 }
            };

            // Spent for 300, get 5% free.
            var discountOnSpent = new List<IProduct>();

            var orderSummary = new Cart()
                .WithItems(cartItems)
                .WithDiscount(new OfferBuyOnItemCount(3, 1, buygetItemfree))
                .WithDiscount(new OfferBuyOnItemCount(1, 1, buyOnegetItemfree))
                .WithDiscount(new OfferBuyDicountOnProduct(5, flatDiscount))
                .WithDiscount(new OfferBuyOnSpent(300, 5, discountOnSpent, true))
                .Build()
                .CheckOut();

            this.WriteOrderSummary(orderSummary);

            Assert.IsNotNull(orderSummary, "Ordersummary retuns null");
            Assert.AreEqual(10, orderSummary.Items.Count, "Order summary count does not match");
            Assert.AreEqual(1979.7050m, orderSummary.TotalAmount, "Order summary amount does not match");
            Assert.AreEqual(392.2950m, orderSummary.DiscountedTotal, "Order summary discount amount does not match");

            Assert.AreEqual(121.8375m, orderSummary.Items[0].GrossPrice, "Order summary eligible item amount does not match");
            Assert.AreEqual(13.1625m, orderSummary.Items[0].DiscountApplied, "Order summary eligible item discount amount does not match");
            Assert.AreEqual(3, orderSummary.Items[0].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual("5% applied 5% off applied on spent of 300 applied ", orderSummary.Items[0].AppliedDiscounts, "Order summary item wormg discount applied");
            
            Assert.AreEqual("5% applied 5% off applied on spent of 300 applied ", orderSummary.Items[1].AppliedDiscounts, "Order summary item wormg discount applied");
            
            Assert.AreEqual("5% applied 5% off applied on spent of 300 applied ", orderSummary.Items[2].AppliedDiscounts, "Order summary item wormg discount applied");
            
            Assert.AreEqual("Buy 3 get 1 applied 5% off applied on spent of 300 applied ", orderSummary.Items[6].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(5, orderSummary.Items[6].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual(1045.0m, orderSummary.Items[6].GrossPrice, "Order summary eligible item amount does not match");
            Assert.AreEqual(330.0m, orderSummary.Items[6].DiscountApplied, "Order summary eligible item discount amount does not match");

            Assert.AreEqual("Buy 1 get 1 applied 5% off applied on spent of 300 applied ", orderSummary.Items[7].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual(1, orderSummary.Items[7].Quantity, "Order summary eligible item discount qiuantityot match");
            Assert.AreEqual(47.5m, orderSummary.Items[7].GrossPrice, "Order summary eligible item amount does not match");
            Assert.AreEqual(2.5m, orderSummary.Items[7].DiscountApplied, "Order summary eligible item discount amount does not match");

            Assert.AreEqual("5% off applied on spent of 300 applied ", orderSummary.Items[8].AppliedDiscounts, "Order summary item wormg discount applied");
            Assert.AreEqual("5% off applied on spent of 300 applied ", orderSummary.Items[9].AppliedDiscounts, "Order summary item wormg discount applied");
        }

        [TestMethod]
        public void BuildingCartWithPassingNoLineitems()
        {
            var spendOnTarget = new List<IProduct>() { };

            var ex = Assert.ThrowsException<System.Exception>(() =>
                new Cart().WithDiscount(new OfferBuyOnSpent(5000, 30, spendOnTarget)).Build());

            Assert.IsTrue(ex.Message == "Order items are not found.");
        }

        private void WriteOrderSummary(OrderSummary orderSummary)
        {
            foreach (var summary in orderSummary.Items.Select((item, i) => new { i, item }))
            {
                if (summary.i == 0)
                {
                    Debug.WriteLine("\n");
                    Debug.WriteLine($"---------------------------------------------------------------------------------------------------------");
                    Debug.WriteLine($"Item | Product Name | Quantity | Discount catagory | Unit price | Total | Discounted amount | Gross total");
                    Debug.WriteLine($"---------------------------------------------------------------------------------------------------------");
                }

                var item = summary.item;
                Debug.WriteLine($"  {summary.i} | {item.ProductName} | {item.Quantity} | {item.AppliedDiscounts} | {item.UnitPrice} | {item.UnitPrice * item.Quantity} | {item.DiscountApplied} | {item.GrossPrice}");

                if (summary.i == orderSummary.Items.Count - 1)
                {
                    Debug.WriteLine($"---------------------------------------------------------------------------------------------------------");
                    Debug.WriteLine($"Total discount {orderSummary.DiscountedTotal}");
                    Debug.WriteLine($"Total price {orderSummary.TotalAmount}");
                    Debug.WriteLine($"---------------------------------------------------------------------------------------------------------");
                    Debug.WriteLine($"Final price {orderSummary.TotalAmount + orderSummary.DiscountedTotal}");
                    Debug.WriteLine($"---------------------------------------------------------------------------------------------------------");
                    Debug.WriteLine("\n");
                }
            }
        }
    }
}
