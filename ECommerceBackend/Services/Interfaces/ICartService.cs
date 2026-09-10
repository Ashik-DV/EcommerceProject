using ECommerceBackend.DTOs.Cart;

namespace ECommerceBackend.Services.Interfaces;

public interface ICartService
{
Task<CartResponseDto> GetCartAsync(int userId);

Task<CartResponseDto> AddToCartAsync(int userId, AddToCartDto dto);

Task<CartResponseDto> UpdateCartItemAsync(
    int userId,
    int cartItemId,
    UpdateCartItemDto dto
);

Task<bool> RemoveCartItemAsync(int userId, int cartItemId);

Task<bool> ClearCartAsync(int userId);

}