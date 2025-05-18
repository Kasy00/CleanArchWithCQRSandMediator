using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Api.Middleware.Exceptions;
using ShoppingCart.Api.Models;
using ShoppingCart.Application.Commands.AddItemToCart;
using ShoppingCart.Application.Commands.CreateCart;
using ShoppingCart.Application.Commands.FinalizeCart;
using ShoppingCart.Application.Commands.RemoveItemFromCart;
using ShoppingCart.Application.Commands.UpdateCartItemQuantity;
using ShoppingCart.Application.Dto;
using ShoppingCart.Application.Queries.GetCart;

namespace ShoppingCart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartsController> _logger;

    public CartsController(IMediator mediator, ILogger<CartsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartDto>> GetCart(Guid id)
    {
        try
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                return Unauthorized("User ID is required");
            }

            if (!Guid.TryParse(userIdHeader, out var userId))
            {
                return BadRequest("Invalid User Id format");
            }

            var query = new GetCartQuery
            {
                CartId = id,
                UserId = userId
            };

            var cart = await _mediator.Send(query);
            return Ok(cart);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting cart {id}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> CreateCart()
    {
        try
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                return Unauthorized("User ID is required");
            }

            if (!Guid.TryParse(userIdHeader, out var userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var command = new CreateCartCommand
            {
                UserId = userId
            };

            var cartId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCart), new { id = cartId }, cartId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cart");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    [HttpPost("{id}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddItemToCart(Guid id, [FromBody] AddItemToCartRequest request)
    {
        try
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                return Unauthorized("User ID is required");
            }

            if (!Guid.TryParse(userIdHeader, out var userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var command = new AddItemToCartCommand
            {
                CartId = id,
                UserId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding item to cart {id}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    [HttpDelete("{id}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> RemoveItemFromCart(Guid id, Guid productId)
    {
        try
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                return Unauthorized("User ID is required");
            }

            if (!Guid.TryParse(userIdHeader, out var userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var command = new RemoveItemFromCartCommand
            {
                CartId = id,
                UserId = userId,
                ProductId = productId
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing item from cart {id}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    [HttpPut("{id}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateCartItemQuantity(Guid id, Guid productId, [FromBody] UpdateCartItemQuantityRequest request)
    {
        try
        {
            // W rzeczywistej aplikacji pobieralibyśmy UserId z tokenu JWT
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                return Unauthorized("User ID is required");
            }

            if (!Guid.TryParse(userIdHeader, out var userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var command = new UpdateCartItemQuantityCommand
            {
                CartId = id,
                UserId = userId,
                ProductId = productId,
                Quantity = request.Quantity
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating item quantity in cart {id}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }

    [HttpPost("{id}/finalize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> FinalizeCart(Guid id)
    {
        try
        {
            // W rzeczywistej aplikacji pobieralibyśmy UserId z tokenu JWT
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                return Unauthorized("User ID is required");
            }

            if (!Guid.TryParse(userIdHeader, out var userId))
            {
                return BadRequest("Invalid User ID format");
            }

            var command = new FinalizeCartCommand
            {
                CartId = id,
                UserId = userId
            };

            var orderId = await _mediator.Send(command);
            return Ok(orderId);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error finalizing cart {id}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }
}