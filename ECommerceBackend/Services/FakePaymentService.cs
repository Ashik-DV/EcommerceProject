using ECommerceBackend.Services.Interfaces;

namespace ECommerceBackend.Services;

public class FakePaymentService : IFakePaymentService
{
// ======================================================
// CREATE FAKE PAYMENT ORDER
// ======================================================

public string CreatePaymentOrder(
    decimal amount,
    int orderId)
{
    return
        $"fake_pay_order_{orderId}_{Guid.NewGuid():N}";
}


// ======================================================
// VERIFY FAKE PAYMENT
// ======================================================

public bool VerifyPayment(
    string paymentOrderId,
    int orderId)
{
    if (string.IsNullOrWhiteSpace(
            paymentOrderId))
    {
        return false;
    }


    return paymentOrderId.StartsWith(
        $"fake_pay_order_{orderId}_",
        StringComparison.Ordinal
    );
}

}