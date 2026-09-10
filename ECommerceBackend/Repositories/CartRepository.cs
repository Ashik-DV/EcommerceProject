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

public Cart? GetCartByUserId(int userId)
{
    var cart =
        _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefault(c => c.UserId == userId);

    return cart;
}

// ======================================================
// CREATE CART
// ======================================================

public Cart CreateCart(Cart cart)
{
    _context.Carts.Add(cart);

    _context.SaveChanges();

    return cart;
}

// ======================================================
// GET CART ITEM
// ======================================================

public CartItem? GetCartItem(
    int cartItemId,
    int userId)
{
    var cartItem =
        _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefault(ci =>
                ci.Id == cartItemId &&
                ci.Cart != null &&
                ci.Cart.UserId == userId);

    return cartItem;
}

// ======================================================
// GET CART ITEM BY PRODUCT ID
// ======================================================

public CartItem? GetCartItemByProductId(
    int userId,
    int productId)
{
    var cartItem =
        _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefault(ci =>
                ci.ProductId == productId &&
                ci.Cart != null &&
                ci.Cart.UserId == userId);

    return cartItem;
}

// ======================================================
// ADD CART ITEM
// ======================================================

public CartItem AddCartItem(
    CartItem cartItem)
{
    _context.CartItems.Add(cartItem);

    _context.SaveChanges();

    return cartItem;
}

// ======================================================
// UPDATE CART ITEM
// ======================================================

public CartItem UpdateCartItem(
    CartItem cartItem)
{
    _context.CartItems.Update(cartItem);

    _context.SaveChanges();

    return cartItem;
}

// ======================================================
// REMOVE CART ITEM
// ======================================================

public bool RemoveCartItem(
    CartItem cartItem)
{
    _context.CartItems.Remove(cartItem);

    _context.SaveChanges();

    return true;
}

// ======================================================
// CLEAR CART
// ======================================================

public bool ClearCart(
    Cart cart)
{
    if (cart.CartItems.Count == 0)
    {
        return true;
    }

    _context.CartItems.RemoveRange(
        cart.CartItems);

    _context.SaveChanges();

    return true;
}

}