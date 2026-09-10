using ECommerceBackend.DTOs.Product;

namespace ECommerceBackend.Services.Interfaces;

public interface IProductService
{
    List<ProductResponseDto> GetAll();

    ProductSearchResponseDto Search(ProductSearchRequest request);

    ProductFilterOptionsDto GetFilterOptions();

    ProductResponseDto? GetById(int id);

    ProductResponseDto Create(ProductCreateDto dto);

    ProductResponseDto? Update(int id, ProductUpdateDto dto);

    bool Delete(int id);

    List<ProductResponseDto> ImportFromCsv(Stream csvStream);
}
