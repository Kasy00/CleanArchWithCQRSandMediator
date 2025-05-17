using MediatR;

namespace ShoppingCart.Application.Commands.CreateCart;

public class CreateCartCommand : IRequest<Guid>
{
    public Guid UserId { get; set;}
}