namespace CRNProductApi.Domain.Entities;

public class Item : BaseEntity
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }

    public Product? Product { get; set; }
}
