using AutoMapper;
using ShoppingCart.Domain.Enums;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Entities;

public class CartEntityToDomainConverter : ITypeConverter<CartEntity, Cart>
{
    public Cart Convert(CartEntity source, Cart destination, ResolutionContext context)
    {
        var cart = Cart.Create(source.UserId);

        typeof(Cart).GetProperty("Id")?.SetValue(cart, source.Id);
        typeof(Cart).GetProperty("Status")?.SetValue(cart, Enum.Parse<CartStatus>(source.Status));
        typeof(Cart).GetProperty("CreatedAt")?.SetValue(cart, source.CreatedAt);
        typeof(Cart).GetProperty("UpdatedAt")?.SetValue(cart, source.UpdatedAt);

        var itemsField = typeof(Cart).GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var items = (List<CartItem>)itemsField?.GetValue(cart)!;

        items.Clear();
        foreach (var itemEntity in source.Items)
        {
            items.Add(new CartItem(itemEntity.ProductId, itemEntity.ProductName, itemEntity.Price, itemEntity.Quantity));
        }

        return cart;
    }
}
