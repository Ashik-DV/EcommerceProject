using ECommerceBackend.Models;

namespace ECommerceBackend.Logging;

public interface IApplicationLogger
{
Task LogAsync(ApplicationLog log);
}
