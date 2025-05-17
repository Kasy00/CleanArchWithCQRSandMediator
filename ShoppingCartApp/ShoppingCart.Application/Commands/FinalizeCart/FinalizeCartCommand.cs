using MediatR;

namespace ShoppingCart.Application.Commands.FinalizeCart;

public class FinalizeCartCommand : IRequest<Guid>
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
}