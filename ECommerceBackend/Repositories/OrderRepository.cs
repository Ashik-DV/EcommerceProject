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

public Order Create(
    Order order)
{
    _context.Orders.Add(order);

    _context.SaveChanges();

    return order;
}

// ======================================================
// UPDATE ORDER
// ======================================================

public Order Update(
    Order order)
{
    _context.Orders.Update(order);

    _context.SaveChanges();

    return order;
}

// ======================================================
// GET USER ORDERS
// ======================================================

public List<Order> GetByUserId(
    int userId)
{
    var orders =
        _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Id)
            .ToList();

    return orders;
}

// ======================================================
// GET SINGLE USER ORDER
// ======================================================

public Order? GetById(
    int id,
    int userId)
{
    var order =
        _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefault(
                o =>
                    o.Id == id &&
                    o.UserId == userId
            );

    return order;
}

// ======================================================
// GET ALL ORDERS - ADMIN
// ======================================================

public List<Order> GetAll()
{
    var orders =
        _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.Id)
            .ToList();

    return orders;
}

// ======================================================
// GET SINGLE ORDER - ADMIN
// ======================================================

public Order? GetByIdForAdmin(
    int id)
{
    var order =
        _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefault(
                o => o.Id == id
            );

    return order;
}

}