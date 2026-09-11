using ECommerceBackend.Data;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly AppDbContext _context;

    public WishlistRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<WishlistItem>> GetByUserIdAsync(int userId)
    {
        return _context.WishlistItems
            .AsNoTracking()
            .Include(item => item.Product)
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();
    }

    public Task<WishlistItem?> GetAsync(int userId, int productId)
    {
        return _context.WishlistItems
            .Include(item => item.Product)
            .FirstOrDefaultAsync(item =>
                item.UserId == userId &&
                item.ProductId == productId);
    }

    public async Task<WishlistItem> AddAsync(WishlistItem item)
    {
        _context.WishlistItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> RemoveAsync(int userId, int productId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(existing =>
                existing.UserId == userId &&
                existing.ProductId == productId);

        if (item == null)
        {
            return false;
        }

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
