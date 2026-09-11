using ECommerceBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Data;

public class AppDbContext : DbContext
{
public AppDbContext(
DbContextOptions<AppDbContext> options)
: base(options)
{
}

public DbSet<User> Users { get; set; }

public DbSet<Product> Products { get; set; }

public DbSet<Cart> Carts { get; set; }

public DbSet<CartItem> CartItems { get; set; }

public DbSet<Order> Orders { get; set; }

public DbSet<OrderItem> OrderItems { get; set; }

public DbSet<WishlistItem> WishlistItems { get; set; }

// ==================================================
// Application Logs
// ==================================================

public DbSet<ApplicationLog> ApplicationLogs { get; set; }


protected override void OnModelCreating(
    ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);


    // ==================================================
    // User → Cart
    // ==================================================

    modelBuilder.Entity<Cart>()
        .HasOne(c => c.User)
        .WithMany()
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);


    // ==================================================
    // Cart → CartItems
    // ==================================================

    modelBuilder.Entity<CartItem>()
        .HasOne(ci => ci.Cart)
        .WithMany(c => c.CartItems)
        .HasForeignKey(ci => ci.CartId)
        .OnDelete(DeleteBehavior.Cascade);


    // ==================================================
    // Product → CartItems
    // ==================================================

    modelBuilder.Entity<CartItem>()
        .HasOne(ci => ci.Product)
        .WithMany()
        .HasForeignKey(ci => ci.ProductId)
        .OnDelete(DeleteBehavior.Restrict);


    // ==================================================
    // CartItem Unique Index
    // One product can appear only once in a cart
    // ==================================================

    modelBuilder.Entity<CartItem>()
        .HasIndex(ci => new
        {
            ci.CartId,
            ci.ProductId
        })
        .IsUnique();


    // ==================================================
    // User → Orders
    // ==================================================

    modelBuilder.Entity<Order>()
        .HasOne(o => o.User)
        .WithMany()
        .HasForeignKey(o => o.UserId)
        .OnDelete(DeleteBehavior.Cascade);


    // ==================================================
    // Order → OrderItems
    // ==================================================

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Order)
        .WithMany(o => o.OrderItems)
        .HasForeignKey(oi => oi.OrderId)
        .OnDelete(DeleteBehavior.Cascade);


    // ==================================================
    // Product → OrderItems
    // ==================================================

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Product)
        .WithMany()
        .HasForeignKey(oi => oi.ProductId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<WishlistItem>()
        .HasOne(item => item.User)
        .WithMany()
        .HasForeignKey(item => item.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<WishlistItem>()
        .HasOne(item => item.Product)
        .WithMany()
        .HasForeignKey(item => item.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<WishlistItem>()
        .HasIndex(item => new
        {
            item.UserId,
            item.ProductId
        })
        .IsUnique();


    // ==================================================
    // OrderItem Price
    // ==================================================

    modelBuilder.Entity<OrderItem>()
        .Property(oi => oi.Price)
        .HasPrecision(18, 2);


    // ==================================================
    // Order TotalAmount
    // ==================================================

    modelBuilder.Entity<Order>()
        .Property(o => o.TotalAmount)
        .HasPrecision(18, 2);


    // ==================================================
    // Product Fields + Search/Filter Indexes
    // ==================================================

    modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasPrecision(18, 2);

    modelBuilder.Entity<Product>()
        .Property(p => p.Category)
        .HasMaxLength(100);

    modelBuilder.Entity<Product>()
        .Property(p => p.Brand)
        .HasMaxLength(100);

    modelBuilder.Entity<Product>()
        .HasIndex(p => p.Category);

    modelBuilder.Entity<Product>()
        .HasIndex(p => p.Brand);

    modelBuilder.Entity<Product>()
        .HasIndex(p => p.Price);

    modelBuilder.Entity<Product>()
        .HasIndex(p => p.StockQuantity);
}

}