using ShoppingCart.Domain.Enums;

namespace ShoppingCart.Domain.Models;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order(Guid cartId, Guid userId, decimal totalAmount)
    {
        Id = Guid.NewGuid();
        CartId = cartId;
        UserId = userId;
        Status = OrderStatus.Created;
        TotalAmount = totalAmount;
        CreatedAt = DateTime.UtcNow;
    }

    public static Order CreateFromCart(Cart cart)
    {
        if (cart.Status != CartStatus.Finalized)
            throw new InvalidOperationException("Cannot create an order from a non-finalized cart");

        return new Order(cart.Id, cart.UserId, cart.TotalValue);
    }
}