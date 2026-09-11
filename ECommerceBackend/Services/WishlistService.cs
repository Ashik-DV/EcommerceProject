using ECommerceBackend.DTOs.Wishlist;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services.Interfaces;

namespace ECommerceBackend.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IProductRepository productRepository)
    {
        _wishlistRepository = wishlistRepository;
        _productRepository = productRepository;
    }

    public async Task<List<WishlistItemResponseDto>> GetAsync(int userId)
    {
        var items = await _wishlistRepository.GetByUserIdAsync(userId);
        return items.Select(Map).ToList();
    }

    public async Task<WishlistItemResponseDto> AddAsync(
        int userId,
        int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var existing = await _wishlistRepository.GetAsync(userId, productId);

        if (existing != null)
        {
            return Map(existing);
        }

        var item = await _wishlistRepository.AddAsync(new WishlistItem
        {
            UserId = userId,
            ProductId = productId,
            Product = product,
            CreatedAt = DateTime.UtcNow
        });

        return Map(item);
    }

    public Task<bool> RemoveAsync(int userId, int productId)
    {
        return _wishlistRepository.RemoveAsync(userId, productId);
    }

    private static WishlistItemResponseDto Map(WishlistItem item)
    {
        return new WishlistItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            Description = item.Product?.Description ?? string.Empty,
            Price = item.Product?.Price ?? 0,
            StockQuantity = item.Product?.StockQuantity ?? 0,
            ImageUrl = item.Product?.ImageUrl ?? string.Empty,
            CreatedAt = item.CreatedAt
        };
    }
}
