using ECommerceBackend.Models;

namespace ECommerceBackend.Services.Interfaces;

public interface IEmailNotificationService
{
    Task SendOrderConfirmationAsync(Order order);
}
