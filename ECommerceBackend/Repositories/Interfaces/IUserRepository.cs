
using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User> CreateAsync(User user);
}