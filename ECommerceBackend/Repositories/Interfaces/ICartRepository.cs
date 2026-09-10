using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface ICartRepository
{
Task<Cart?> GetCartByUserIdAsync(int userId);

Task<Cart> CreateCartAsync(Cart cart);

Task<CartItem?> GetCartItemAsync(int cartItemId, int userId);

Task<CartItem?> GetCartItemByProductIdAsync(int userId, int productId);

Task<CartItem> AddCartItemAsync(CartItem cartItem);

Task<CartItem> UpdateCartItemAsync(CartItem cartItem);

Task<bool> RemoveCartItemAsync(CartItem cartItem);

Task<bool> ClearCartAsync(Cart cart);

}