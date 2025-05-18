using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Api.Models;

public class UpdateCartItemQuantityRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}