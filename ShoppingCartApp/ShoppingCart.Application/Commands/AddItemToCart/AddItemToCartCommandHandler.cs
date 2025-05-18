using MediatR;
using ShoppingCart.Api.Middleware.Exceptions;
using ShoppingCart.Domain.Interfaces;

namespace ShoppingCart.Application.Commands.AddItemToCart;

public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, Unit>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductService _productService;

    public AddItemToCartCommandHandler(ICartRepository cartRepository, IProductService productService)
    {
        _cartRepository = cartRepository;
        _productService = productService;
    }

    public async Task<Unit> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetById(request.CartId);
        if (cart == null)
            throw new NotFoundException($"Cart with ID {request.CartId} not found");

        if (cart.UserId != request.UserId)
            throw new UnauthorizedAccessException("You don't have access to this cart");

        var product = await _productService.GetProductById(request.ProductId);
        if (product == null)
            throw new NotFoundException($"Product with ID {request.ProductId} not found");

        if (!product.IsAvailable)
            throw new InvalidOperationException($"Product {product.Name} is not available");

        var isAvailable = await _productService.IsProductAvailable(request.ProductId, request.Quantity);
        if (!isAvailable)
            throw new InvalidOperationException($"Requested quantity of product {product.Name} is not available");

        cart.AddItem(product.Id, product.Name, product.Price, request.Quantity);

        await _cartRepository.Update(cart);

        return Unit.Value;
    }
}