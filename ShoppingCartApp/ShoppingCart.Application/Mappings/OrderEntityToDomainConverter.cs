using AutoMapper;
using ShoppingCart.Domain.Enums;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Entities;

public class OrderEntityToDomainConverter : ITypeConverter<OrderEntity, Order>
{
    public Order Convert(OrderEntity source, Order destination, ResolutionContext context)
    {
        var ctor = typeof(Order).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0];
        var order = (Order)ctor.Invoke(new object[] { source.CartId, source.UserId, source.TotalAmount });

        typeof(Order).GetProperty("Id")?.SetValue(order, source.Id);
        typeof(Order).GetProperty("Status")?.SetValue(order, Enum.Parse<OrderStatus>(source.Status));
        typeof(Order).GetProperty("CreatedAt")?.SetValue(order, source.CreatedAt);

        return order;
    }
}
