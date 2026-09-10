using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.DTOs.Cart;

public class UpdateCartItemDto
{
[Range(1, int.MaxValue)]
public int Quantity { get; set; }
}