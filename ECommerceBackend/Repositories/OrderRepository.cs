using ECommerceBackend.Data;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Repositories;

public class OrderRepository : IOrderRepository
{
private readonly AppDbContext _context;

public OrderRepository(AppDbContext context)
{
    _context = context;
}

// ======================================================
// CREATE ORDER
// ======================================================

public async Task<Order> CreateAsync(
    Order order)
{
    _context.Orders.Add(order);

    await _context.SaveChangesAsync();

    return order;
}

// ======================================================
// UPDATE ORDER
// ======================================================

public async Task<Order> UpdateAsync(
    Order order)
{
    _context.Orders.Update(order);

    await _context.SaveChangesAsync();

    return order;
}

// ======================================================
// GET USER ORDERS
// ======================================================

public async Task<List<Order>> GetByUserIdAsync(
    int userId)
{
    var orders =
        _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Id)
            .ToListAsync();

    return await orders;
}

// ======================================================
// GET SINGLE USER ORDER
// ======================================================

public async Task<Order?> GetByIdAsync(
    int id,
    int userId)
{
    var order =
        _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(
                o =>
                    o.Id == id &&
                    o.UserId == userId
            );

    return await order;
}

// ======================================================
// GET ALL ORDERS - ADMIN
// ======================================================

public async Task<List<Order>> GetAllAsync()
{
    var orders =
        _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.Id)
            .ToListAsync();

    return await orders;
}

// ======================================================
// GET SINGLE ORDER - ADMIN
// ======================================================

public async Task<Order?> GetByIdForAdminAsync(
    int id)
{
    var order =
        _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(
                o => o.Id == id
            );

    return await order;
}

}