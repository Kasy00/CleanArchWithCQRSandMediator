using ShoppingCart.Application.Dto;

namespace ShoppingCart.Domain.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetProductById(Guid productId);
    Task<bool> IsProductAvailable(Guid productId, int quantity);
}