using System.Data;
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
    private readonly int _maxRetries = 3;

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
        int retryCount = 0;
        bool success = false;

        while (!success && retryCount < _maxRetries)
        {
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

                UpdateCartItems(existingCart, cart);

                await _context.SaveChangesAsync();
                success = true;

                _logger.LogInformation($"Cart {cart.Id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;

                if (retryCount >= _maxRetries)
                {
                    _logger.LogError(ex, $"Failed to update cart {cart.Id} after {_maxRetries} attempts due to concurrency conflicts");
                    throw new DBConcurrencyException($"Cart with ID {cart.Id} was modified by another process and update failed after {_maxRetries} attempts");
                }

                _context.ChangeTracker.Clear();

                await Task.Delay(50 * retryCount);

                _logger.LogWarning($"Concurrency conflict detected for cart {cart.Id}. Retry attempt {retryCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart {cart.Id}");
                throw;
            }
        }
    }

    private void UpdateCartItems(CartEntity existingCart, Cart cart)
    {
        var itemsToRemove = existingCart.Items
            .Where(existingItem => !cart.Items.Any(item => item.ProductId == existingItem.ProductId))
            .ToList();

        foreach (var itemToRemove in itemsToRemove)
        {
            existingCart.Items.Remove(itemToRemove);
            _context.CartItems.Remove(itemToRemove);
        }

        foreach (var item in cart.Items)
        {
            var existingItem = existingCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem == null)
            {
                var newItem = new CartItemEntity
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity
                };

                existingCart.Items.Add(newItem);
            }
            else
            {
                existingItem.ProductName = item.ProductName;
                existingItem.Price = item.Price;
                existingItem.Quantity = item.Quantity;
            }
        }
    }
}