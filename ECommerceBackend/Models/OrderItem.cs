namespace ECommerceBackend.Models;

public class OrderItem
{
public int Id { get; set; }

public int OrderId { get; set; }

public int ProductId { get; set; }

public int Quantity { get; set; }

// Price at the time the order was placed
public decimal Price { get; set; }

// Relationships
public Order Order { get; set; } = null!;

public Product Product { get; set; } = null!;

}