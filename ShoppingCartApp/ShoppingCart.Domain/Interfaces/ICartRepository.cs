using ShoppingCart.Domain.Models;

namespace ShoppingCart.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart> GetById(Guid id);
    Task<IEnumerable<Cart>> GetByUserId(Guid userId);
    Task Add(Cart cart);
    Task Update(Cart cart);
    Task<Cart> GetActiveCartByUserId(Guid userId);
}