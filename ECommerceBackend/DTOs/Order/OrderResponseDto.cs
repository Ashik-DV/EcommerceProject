namespace ECommerceBackend.DTOs.Order;

public class OrderResponseDto
{
public int Id { get; set; }

public int UserId { get; set; }

public decimal TotalAmount { get; set; }

public string Status { get; set; } = string.Empty;

public string ShippingAddress { get; set; } = string.Empty;

public DateTime CreatedAt { get; set; }

public List<OrderItemResponseDto> Items { get; set; }
    = new List<OrderItemResponseDto>();

}