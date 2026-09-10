using Razorpay.Api;

namespace ECommerceBackend.Services.Interfaces;

public interface IRazorpayService
{
Order CreateOrder(
decimal amount,
string receipt
);

bool VerifyPaymentSignature(
    string razorpayOrderId,
    string razorpayPaymentId,
    string razorpaySignature
);

}