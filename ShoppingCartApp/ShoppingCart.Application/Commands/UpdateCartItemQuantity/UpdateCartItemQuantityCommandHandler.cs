using MediatR;
using ShoppingCart.Api.Middleware.Exceptions;
using ShoppingCart.Domain.Interfaces;

namespace ShoppingCart.Application.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, Unit>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductService _productService;

    public UpdateCartItemQuantityCommandHandler(ICartRepository cartRepository, IProductService productService)
    {
        _cartRepository = cartRepository;
        _productService = productService;
    }

    public async Task<Unit> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetById(request.CartId);
        if (cart == null)
            throw new NotFoundException($"Cart with ID {request.CartId} not found");

        if (cart.UserId != request.UserId)
            throw new UnauthorizedAccessException("You don't have access to this cart");

        if (request.Quantity <= 0)
        {
            cart.RemoveItem(request.ProductId);
        }
        else
        {
            var isAvailable = await _productService.IsProductAvailable(request.ProductId, request.Quantity);
            if (!isAvailable)
                throw new InvalidOperationException($"Requested quantity is not available");

            cart.UpdateItemQuantity(request.ProductId, request.Quantity);
        }
        
        await _cartRepository.Update(cart);
        
        return Unit.Value;
    }
}