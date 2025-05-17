using AutoMapper;
using MediatR;
using ShoppingCart.Application.Dto;
using ShoppingCart.Domain.Interfaces;

namespace ShoppingCart.Application.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetCartQueryHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetById(request.CartId);
        if (cart == null)
            throw new NotFoundException($"Cart with ID {request.CartId} not found");

        if (cart.UserId != request.UserId)
            throw new UnauthorizedAccessException("You don't have access to this cart");

        return _mapper.Map<CartDto>(cart);
    }
}