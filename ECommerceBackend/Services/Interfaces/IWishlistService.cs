using ECommerceBackend.DTOs.Wishlist;

namespace ECommerceBackend.Services.Interfaces;

public interface IWishlistService
{
    Task<List<WishlistItemResponseDto>> GetAsync(int userId);

    Task<WishlistItemResponseDto> AddAsync(int userId, int productId);

    Task<bool> RemoveAsync(int userId, int productId);
}
