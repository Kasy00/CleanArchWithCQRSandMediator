using MediatR;

namespace ShoppingCart.Application.Commands.RemoveItemFromCart;

public class RemoveItemFromCartCommand : IRequest<Unit>
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
}