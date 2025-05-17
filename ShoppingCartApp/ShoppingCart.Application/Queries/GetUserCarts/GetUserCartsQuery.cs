using MediatR;
using ShoppingCart.Application.Dto;

namespace ShoppingCart.Application.Queries.GetUserCart;

public class GetUserCartsQuery : IRequest<List<CartSummaryDto>>
{
    public Guid UserId { get; set; }
}