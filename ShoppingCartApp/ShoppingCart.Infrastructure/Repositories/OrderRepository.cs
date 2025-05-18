using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Infrastructure.Entities;

namespace ShoppingCart.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Order> GetById(Guid id)
    {
        var orderEntity = await _context.Orders.FindAsync(id);
        if (orderEntity == null)
            return null;

        return _mapper.Map<Order>(orderEntity);
    }

    public async Task<IEnumerable<Order>> GetByUserId(Guid userId)
    {
        var orderEntities = await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<Order>>(orderEntities);
    }

    public async Task Add(Order order)
    {
        var orderEntity = _mapper.Map<OrderEntity>(order);
        await _context.Orders.AddAsync(orderEntity);
        await _context.SaveChangesAsync();
    }
}