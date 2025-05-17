using MediatR;
using ShoppingCart.Domain.Interfaces;

namespace ShoppingCart.Application.Commands.RemoveItemFromCart;

public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, Unit>
{
    private readonly ICartRepository _cartRepository;

    public RemoveItemFromCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Unit> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetById(request.CartId);
        if (cart == null)
            throw new NotFoundException($"Cart with ID {request.CartId} not found");

        if (cart.UserId != request.UserId)
            throw new UnauthorizedAccessException("You don't have access to this cart");

        cart.RemoveItem(request.ProductId);

        await _cartRepository.Update(cart);

        return Unit.Value;
    }
}