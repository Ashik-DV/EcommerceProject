using Razorpay.Api;
using ECommerceBackend.Services.Interfaces;

namespace ECommerceBackend.Services;

public class RazorpayService : IRazorpayService
{
private readonly string _keyId;
private readonly string _keySecret;

public RazorpayService(
    IConfiguration configuration)
{
    _keyId =
        configuration["Razorpay:KeyId"]
        ?? throw new InvalidOperationException(
            "Razorpay KeyId is missing."
        );

    _keySecret =
        configuration["Razorpay:KeySecret"]
        ?? throw new InvalidOperationException(
            "Razorpay KeySecret is missing."
        );
}

// ======================================================
// CREATE RAZORPAY ORDER
// ======================================================

public Order CreateOrder(
    decimal amount,
    string receipt)
{
    var client =
        new RazorpayClient(
            _keyId,
            _keySecret
        );

    var options =
        new Dictionary<string, object>
        {
            {
                "amount",
                (int)(amount * 100)
            },

            {
                "currency",
                "INR"
            },

            {
                "receipt",
                receipt
            },

            {
                "payment_capture",
                1
            }
        };

    var order =
        client.Order.Create(
            options
        );

    return order;
}

// ======================================================
// VERIFY PAYMENT SIGNATURE
// ======================================================

public bool VerifyPaymentSignature(
    string razorpayOrderId,
    string razorpayPaymentId,
    string razorpaySignature)
{
    try
    {
        var attributes =
            new Dictionary<string, string>
            {
                {
                    "razorpay_order_id",
                    razorpayOrderId
                },

                {
                    "razorpay_payment_id",
                    razorpayPaymentId
                },

                {
                    "razorpay_signature",
                    razorpaySignature
                }
            };

        Utils.verifyPaymentSignature(
            attributes
        );

        return true;
    }
    catch
    {
        return false;
    }
}

}