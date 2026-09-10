using ECommerceBackend.DTOs.Order;

namespace ECommerceBackend.Services.Interfaces;

public interface IOrderService
{
OrderCreateResponseDto CreateOrder(
int userId,
OrderCreateDto dto
);

bool VerifyPayment(
    int userId,
    FakePaymentDto dto
);

List<OrderResponseDto> GetMyOrders(
    int userId
);

OrderResponseDto? GetMyOrderById(
    int userId,
    int orderId
);

List<OrderResponseDto> GetAllOrders();

OrderResponseDto? GetOrderByIdForAdmin(
    int orderId
);

}