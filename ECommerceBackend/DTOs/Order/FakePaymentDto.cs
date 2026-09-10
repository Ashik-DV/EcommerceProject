using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.DTOs.Order;

public class FakePaymentDto
{
[Required]
public int OrderId { get; set; }

[Required]
public string PaymentOrderId { get; set; }
    = string.Empty;

[Required]
public string PaymentMethod { get; set; }
    = string.Empty;

}