using ECommerceBackend.DTOs.Cart;

namespace ECommerceBackend.Services.Interfaces;

public interface ICartService
{
CartResponseDto GetCart(int userId);

CartResponseDto AddToCart(int userId, AddToCartDto dto);

CartResponseDto UpdateCartItem(
    int userId,
    int cartItemId,
    UpdateCartItemDto dto
);

bool RemoveCartItem(int userId, int cartItemId);

bool ClearCart(int userId);

}