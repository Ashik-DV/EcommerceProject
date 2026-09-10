using ECommerceBackend.DTOs.Product;

namespace ECommerceBackend.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();

    Task<ProductSearchResponseDto> SearchAsync(ProductSearchRequest request);

    Task<ProductFilterOptionsDto> GetFilterOptionsAsync();

    Task<ProductResponseDto?> GetByIdAsync(int id);

    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);

    Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);

    Task<bool> DeleteAsync(int id);

    Task<List<ProductResponseDto>> ImportFromCsvAsync(Stream csvStream);
}
