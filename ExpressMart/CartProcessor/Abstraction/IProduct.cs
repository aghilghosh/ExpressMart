namespace SmartCart.Abstraction
{
    public interface IProduct
    {
        string ProductName { get; set; }
        int ProductCode { get; set; }
        decimal Price { get; set; }
    }
}
