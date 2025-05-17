namespace ShoppingCart.Infrastructure.Entities;

public class OrderEntity
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public required string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}