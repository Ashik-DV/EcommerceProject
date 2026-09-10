using ECommerceBackend.DTOs.Product;
using ECommerceBackend.Models;

namespace ECommerceBackend.Repositories.Interfaces;

public interface IProductRepository
{
    List<Product> GetAll();

    ProductSearchResult Search(ProductSearchRequest request);

    ProductFilterOptionsDto GetFilterOptions();

    Product? GetById(int id);

    Product Create(Product product);

    Product? Update(int id, Product product);

    bool Delete(int id);
}
