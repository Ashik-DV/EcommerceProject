using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);

    User Create(User user);
}