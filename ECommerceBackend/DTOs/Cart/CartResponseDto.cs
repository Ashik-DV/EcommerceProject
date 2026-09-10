namespace ECommerceBackend.DTOs.Cart;

public class CartResponseDto
{
public int CartId { get; set; }

public int UserId { get; set; }

public List<CartItemResponseDto> Items { get; set; } = new();

public decimal TotalAmount { get; set; }

public int TotalItems { get; set; }

}