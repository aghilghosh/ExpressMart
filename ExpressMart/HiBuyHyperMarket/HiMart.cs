using SmartCart.Abstraction;

namespace SmartCart
{

    public class HiBuyProducts : IProduct
    {
        public string ProductName { get; set; }
        public int ProductCode { get; set; }
        public decimal Price { get; set; }
    }
}
