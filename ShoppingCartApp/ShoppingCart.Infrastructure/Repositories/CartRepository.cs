using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Middleware.Exceptions;
using ShoppingCart.Domain.Enums;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Infrastructure.Entities;

namespace ShoppingCart.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<CartRepository> _logger;
    private readonly IMapper _mapper;

    public CartRepository(AppDbContext context, ILogger<CartRepository> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Cart> GetById(Guid id)
    {
        var cartEntity = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cartEntity == null)
            return null;

        return _mapper.Map<Cart>(cartEntity);
    }

    public async Task<IEnumerable<Cart>> GetByUserId(Guid userId)
    {
        var cartEntities = await _context.Carts
            .Include(c => c.Items)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<Cart>>(cartEntities);
    }

    public async Task<Cart?> GetActiveCartByUserId(Guid userId)
    {
        var cartEntity = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.New.ToString());

        return cartEntity == null ? null : _mapper.Map<Cart>(cartEntity);
    }

    public async Task Add(Cart cart)
    {
        var cartEntity = _mapper.Map<CartEntity>(cart);
        await _context.Carts.AddAsync(cartEntity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Cart cart)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

        try
        {
            var existingCart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (existingCart == null)
            {
                throw new NotFoundException($"Cart with ID {cart.Id} not found during update");
            }

            existingCart.Status = cart.Status.ToString();
            existingCart.UpdatedAt = cart.UpdatedAt;

            _context.CartItems.RemoveRange(existingCart.Items);

            existingCart.Items = cart.Items.Select(i => new CartItemEntity
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation($"Cart {cart.Id} updated successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, $"Error updating cart {cart.Id}");
            throw;
        }
    }
}