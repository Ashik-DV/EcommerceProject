using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IOrderRepository
{
Order Create(
Order order
);

Order Update(
    Order order
);

List<Order> GetByUserId(
    int userId
);

Order? GetById(
    int id,
    int userId
);

List<Order> GetAll();

Order? GetByIdForAdmin(
    int id
);

}