using MediatR;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;

namespace ShoppingCart.Application.Commands.FinalizeCart;

public class FinalizeCartCommandHandler : IRequestHandler<FinalizeCartCommand, Guid>
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;

    public FinalizeCartCommandHandler(ICartRepository cartRepository, IOrderRepository orderRepository)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handler(FinalizeCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetById(request.CartId);
        if (cart == null)
            throw new NotFoundException($"Cart with ID {request.CartId} not found");

        if (cart.UserId != request.UserId)
            throw new UnauthorizedAccessException("You don't have access to this cart");

        cart.FinalizeCart();

        await _cartRepository.Update(cart);

        var order = Order.CreateFromCart(cart);

        await _orderRepository.Add(order);

        return order.Id;
    }
}