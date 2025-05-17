using AutoMapper;
using ShoppingCart.Application.Dto;
using ShoppingCart.Domain.Models;

namespace ShoppingCart.Application.Mappings.CartProfile;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<CartItem, CartItemDto>();
        CreateMap<Cart, CartSummaryDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));
    }
}