namespace ShoppingCart.Application.Dto;

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}