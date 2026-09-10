using ECommerceBackend.Models;

namespace ECommerceBackend.Services.Interfaces;

public interface IFakePaymentService
{
string CreatePaymentOrder(
decimal amount,
int orderId
);

bool VerifyPayment(
    string paymentOrderId,
    int orderId
);

}