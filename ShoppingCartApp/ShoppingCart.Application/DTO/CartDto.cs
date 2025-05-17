namespace ShoppingCart.Application.Dto;

public class CartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Status { get; set; }
    public List<CartItemDto>? Items { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}