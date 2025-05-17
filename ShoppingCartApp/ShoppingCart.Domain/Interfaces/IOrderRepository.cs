using ShoppingCart.Domain.Models;

namespace ShoppingCart.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetById(Guid id);
    Task<IEnumerable<Order>> GetByUserId(Guid userId);
    Task Add(Order order);
}