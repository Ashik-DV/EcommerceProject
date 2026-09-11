using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IWishlistRepository
{
    Task<List<WishlistItem>> GetByUserIdAsync(int userId);

    Task<WishlistItem?> GetAsync(int userId, int productId);

    Task<WishlistItem> AddAsync(WishlistItem item);

    Task<bool> RemoveAsync(int userId, int productId);
}
