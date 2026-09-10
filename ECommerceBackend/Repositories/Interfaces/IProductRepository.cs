using ECommerceBackend.DTOs.Product;
using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<ProductSearchResult> SearchAsync(ProductSearchRequest request);

    Task<ProductFilterOptionsDto> GetFilterOptionsAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<Product> CreateAsync(Product product);

    Task<Product?> UpdateAsync(int id, Product product);

    Task<bool> DeleteAsync(int id);
}
