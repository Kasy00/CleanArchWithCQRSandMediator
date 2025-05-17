using MediatR;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;

namespace ShoppingCart.Application.Commands.CreateCart;

public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, Guid>
{
    private readonly ICartRepository _cartRepository;

    public CreateCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Guid> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        var existingCart = await _cartRepository.GetActiveCartByUserId(request.UserId);
        if (existingCart != null)
        {
            return existingCart.Id;
        }

        var cart = Cart.Create(request.UserId);
        await _cartRepository.Add(cart);
        return cart.Id;
    }
}