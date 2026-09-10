using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IOrderRepository
{
Task<Order> CreateAsync(
Order order
);

Task<Order> UpdateAsync(
    Order order
);

Task<List<Order>> GetByUserIdAsync(
    int userId
);

Task<Order?> GetByIdAsync(
    int id,
    int userId
);

Task<List<Order>> GetAllAsync();

Task<Order?> GetByIdForAdminAsync(
    int id
);

}