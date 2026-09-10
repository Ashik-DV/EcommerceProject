using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface ICartRepository
{
Cart? GetCartByUserId(int userId);

Cart CreateCart(Cart cart);

CartItem? GetCartItem(int cartItemId, int userId);

CartItem? GetCartItemByProductId(int userId, int productId);

CartItem AddCartItem(CartItem cartItem);

CartItem UpdateCartItem(CartItem cartItem);

bool RemoveCartItem(CartItem cartItem);

bool ClearCart(Cart cart);

}