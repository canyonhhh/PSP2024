namespace PSPOS.ServiceDefaults.Models;

public class ProductStock : BaseClass
{
    public ProductStock(int quantity, Guid productId)
    {
        Quantity = quantity;
        ProductId = productId;
    }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
}