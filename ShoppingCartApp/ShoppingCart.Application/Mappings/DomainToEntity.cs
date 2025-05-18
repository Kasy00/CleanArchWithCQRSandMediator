namespace ShoppingCart.Application.Mappings.DomainToEntity;

using AutoMapper;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Entities;

public class DomainToEntityProfile : Profile
{
    public DomainToEntityProfile()
    {
        CreateMap<CartEntity, Cart>().ConvertUsing<CartEntityToDomainConverter>();
        CreateMap<OrderEntity, Order>().ConvertUsing<OrderEntityToDomainConverter>();

        CreateMap<Cart, CartEntity>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new CartItemEntity
            {
                Id = Guid.NewGuid(),
                CartId = src.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()));

        CreateMap<Order, OrderEntity>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
