using ECommerceBackend.Data;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;

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

public User? GetByEmail(string email)
{
    var user =
        _context.Users
            .FirstOrDefault(x => x.Email == email);

    return user;
}

// ======================================================
// CREATE USER
// ======================================================

public User Create(User user)
{
    _context.Users.Add(user);

    _context.SaveChanges();

    return user;
}

}