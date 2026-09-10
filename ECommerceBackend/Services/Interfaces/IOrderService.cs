using ECommerceBackend.DTOs.Order;

namespace ECommerceBackend.Services.Interfaces;

public interface IOrderService
{
Task<OrderCreateResponseDto> CreateOrderAsync(
int userId,
OrderCreateDto dto
);

Task<bool> VerifyPaymentAsync(
    int userId,
    FakePaymentDto dto
);

Task<List<OrderResponseDto>> GetMyOrdersAsync(
    int userId
);

Task<OrderResponseDto?> GetMyOrderByIdAsync(
    int userId,
    int orderId
);

Task<List<OrderResponseDto>> GetAllOrdersAsync();

Task<OrderResponseDto?> GetOrderByIdForAdminAsync(
    int orderId
);

}