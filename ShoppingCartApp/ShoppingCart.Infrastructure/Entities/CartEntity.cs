namespace ShoppingCart.Infrastructure.Entities;

public class CartEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Status { get; set; }
    public List<CartItemEntity> Items { get; set; } = new List<CartItemEntity>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}