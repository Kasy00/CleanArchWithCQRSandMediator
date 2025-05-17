namespace ShoppingCart.Application.Dto;

public class CartSummaryDto
{
    public Guid Id { get; set; }
    public required string Status { get; set; }
    public int ItemsCount { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreatedAt { get; set; }
}