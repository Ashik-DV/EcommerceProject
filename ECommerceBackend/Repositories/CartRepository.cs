using ECommerceBackend.Data;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Repositories;

public class CartRepository : ICartRepository
{
private readonly AppDbContext _context;

public CartRepository(AppDbContext context)
{
    _context = context;
}

// ======================================================
// GET CART BY USER ID
// ======================================================

public async Task<Cart?> GetCartByUserIdAsync(int userId)
{
    var cart =
        _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    return await cart;
}

// ======================================================
// CREATE CART
// ======================================================

public async Task<Cart> CreateCartAsync(Cart cart)
{
    _context.Carts.Add(cart);

    await _context.SaveChangesAsync();

    return cart;
}

// ======================================================
// GET CART ITEM
// ======================================================

public async Task<CartItem?> GetCartItemAsync(
    int cartItemId,
    int userId)
{
    var cartItem =
        _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci =>
                ci.Id == cartItemId &&
                ci.Cart != null &&
                ci.Cart.UserId == userId);

    return await cartItem;
}

// ======================================================
// GET CART ITEM BY PRODUCT ID
// ======================================================

public async Task<CartItem?> GetCartItemByProductIdAsync(
    int userId,
    int productId)
{
    var cartItem =
        _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci =>
                ci.ProductId == productId &&
                ci.Cart != null &&
                ci.Cart.UserId == userId);

    return await cartItem;
}

// ======================================================
// ADD CART ITEM
// ======================================================

public async Task<CartItem> AddCartItemAsync(
    CartItem cartItem)
{
    _context.CartItems.Add(cartItem);

    await _context.SaveChangesAsync();

    return cartItem;
}

// ======================================================
// UPDATE CART ITEM
// ======================================================

public async Task<CartItem> UpdateCartItemAsync(
    CartItem cartItem)
{
    _context.CartItems.Update(cartItem);

    await _context.SaveChangesAsync();

    return cartItem;
}

// ======================================================
// REMOVE CART ITEM
// ======================================================

public async Task<bool> RemoveCartItemAsync(
    CartItem cartItem)
{
    _context.CartItems.Remove(cartItem);

    await _context.SaveChangesAsync();

    return true;
}

// ======================================================
// CLEAR CART
// ======================================================

public async Task<bool> ClearCartAsync(
    Cart cart)
{
    if (cart.CartItems.Count == 0)
    {
        return true;
    }

    _context.CartItems.RemoveRange(
        cart.CartItems);

    await _context.SaveChangesAsync();

    return true;
}

}