using ShoppingCart.Domain.Enums;

namespace ShoppingCart.Domain.Models;

public class Cart
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public CartStatus Status { get; private set; }
    private readonly List<CartItem> _items;
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
    public decimal TotalValue => _items.Sum(item => item.Quantity * item.Price);
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Cart(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Status = CartStatus.New;
        _items = new List<CartItem>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Cart Create(Guid userId)
    {
        return new Cart(userId);
    }

    public void AddItem(Guid productId, string productName, decimal price, int quantity)
    {
        if (Status != CartStatus.New)
            throw new InvalidOperationException("Cannot add items to a finalized cart");

        var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new CartItem(productId, productName, price, quantity));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != CartStatus.New)
            throw new InvalidOperationException("Cannot remove items from a finalized cart");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException($"Product {productId} not found in cart");

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (Status != CartStatus.New)
            throw new InvalidOperationException("Cannot update items in a finalized cart");

        if (quantity <= 0)
        {
            RemoveItem(productId);
            return;
        }

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException($"Product {productId} not found in cart");

        item.UpdateQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void FinalizeCart()
    {
        if (Status != CartStatus.New)
            throw new InvalidOperationException("Cart is already finalized");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot finalize an empty cart");

        Status = CartStatus.Finalized;
        UpdatedAt = DateTime.UtcNow;
    }
}