using MediatR;
using ShoppingCart.Application.Dto;

namespace ShoppingCart.Application.Queries.GetCart;

public class GetCartQuery : IRequest<CartDto>
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
}