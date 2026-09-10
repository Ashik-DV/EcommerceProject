using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.DTOs.Order;

public class OrderCreateDto
{
[Required]
public string ShippingAddress { get; set; } = string.Empty;
}