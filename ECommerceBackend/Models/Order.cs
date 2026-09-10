using System.ComponentModel.DataAnnotations;

namespace ECommerceBackend.Models;

public class Order
{
[Key]
public int Id { get; set; }

public int UserId { get; set; }

public decimal TotalAmount { get; set; }

public string Status { get; set; } = "Pending";

public string ShippingAddress { get; set; } = string.Empty;

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

// ======================================================
// PAYMENT INFORMATION
// ======================================================

public string? PaymentOrderId { get; set; }

public string? PaymentId { get; set; }

// ======================================================
// RELATIONSHIP WITH USER
// ======================================================

public User User { get; set; } = null!;

// ======================================================
// RELATIONSHIP WITH ORDER ITEMS
// ======================================================

public ICollection<OrderItem> OrderItems { get; set; }
    = new List<OrderItem>();

}