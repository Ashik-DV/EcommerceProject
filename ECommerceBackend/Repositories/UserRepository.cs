using ECommerceBackend.Data;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Repositories;

public class UserRepository : IUserRepository
{
private readonly AppDbContext _context;

public UserRepository(AppDbContext context)
{
    _context = context;
}

// ======================================================
// GET USER BY EMAIL
// ======================================================

public async Task<User?> GetByEmailAsync(string email)
{
    var user =
        await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

    return user;
}

// ======================================================
// CREATE USER
// ======================================================

public async Task<User> CreateAsync(User user)
{
    _context.Users.Add(user);

    await _context.SaveChangesAsync();

    return user;
}

}