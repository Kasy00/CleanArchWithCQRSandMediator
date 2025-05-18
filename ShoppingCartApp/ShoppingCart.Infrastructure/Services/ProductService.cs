using System.Text.Json;
using Microsoft.Extensions.Options;
using ShoppingCart.Application.Dto;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Infrastructure.Options;

namespace ShoppingCart.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;
    private readonly IOptions<ProductServiceOptions> _options;


    public ProductService(HttpClient httpClient, ILogger<ProductService> logger, IOptions<ProductServiceOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options;

        _httpClient.BaseAddress = new Uri(_options.Value.BaseUrl);
    }

    public async Task<ProductDto> GetProductById(Guid productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/{productId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return product;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Product with ID {productId} not found");
                return null;
            }

            _logger.LogError($"Error getting product {productId}: {response.StatusCode}");
            throw new Exception($"Error getting product: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception when getting product {productId}");
            throw;
        }
    }

    public async Task<bool> IsProductAvailable(Guid productId, int quantity)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/{productId}/availability?quantity={quantity}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var availability = JsonSerializer.Deserialize<ProductAvailabilityDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return availability.IsAvailable;
            }

            _logger.LogError($"Error checking product availability {productId}: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception when checking product availability {productId}");
            return false;
        }
    }
}