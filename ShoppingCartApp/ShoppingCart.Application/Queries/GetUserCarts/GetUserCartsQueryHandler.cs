using AutoMapper;
using MediatR;
using ShoppingCart.Application.Dto;
using ShoppingCart.Domain.Interfaces;

namespace ShoppingCart.Application.Queries.GetUserCart;

public class GetUserCartsQueryHandler : IRequestHandler<GetUserCartsQuery, List<CartSummaryDto>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetUserCartsQueryHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<List<CartSummaryDto>> Handle(GetUserCartsQuery request, CancellationToken cancellationToken)
    {
        var carts = await _cartRepository.GetByUserId(request.UserId);

        return _mapper.Map<List<CartSummaryDto>>(carts);
    }
}