using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.Models;

public class Cart
{
[Key]
public int Id { get; set; }

public int UserId { get; set; }

public User? User { get; set; }

public List<CartItem> CartItems { get; set; } = new();

}