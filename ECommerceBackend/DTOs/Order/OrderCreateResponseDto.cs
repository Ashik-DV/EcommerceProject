using System;

namespace ECommerceBackend.DTOs.Order;

public class OrderCreateResponseDto
{
public int OrderId { get; set; }

public decimal Amount { get; set; }

public string Currency { get; set; } = "INR";

public string PaymentOrderId { get; set; } = string.Empty;

}