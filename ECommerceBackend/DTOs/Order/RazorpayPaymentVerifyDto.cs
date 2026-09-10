using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.DTOs.Order;

public class RazorpayPaymentVerifyDto
{
[Required]
public int OrderId { get; set; }

[Required]
public string RazorpayOrderId { get; set; }
    = string.Empty;

[Required]
public string RazorpayPaymentId { get; set; }
    = string.Empty;

[Required]
public string RazorpaySignature { get; set; }
    = string.Empty;

}