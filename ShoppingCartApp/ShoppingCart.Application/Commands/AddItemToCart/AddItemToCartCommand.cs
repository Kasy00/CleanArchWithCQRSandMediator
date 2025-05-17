using MediatR;

namespace ShoppingCart.Application.Commands.AddItemToCart;

public class AddItemToCartCommand : IRequest<Unit>
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}