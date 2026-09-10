using CsvHelper;
using ECommerceBackend.DTOs.Product;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services.Interfaces;
using System.Globalization;

namespace ECommerceBackend.Services;

public class ProductService : IProductService
{
private readonly IProductRepository _repository;

public ProductService(IProductRepository repository)
{
    _repository = repository;
}

public List<ProductResponseDto> GetAll()
{
    return _repository.GetAll()
        .Select(Map)
        .ToList();
}

public ProductSearchResponseDto Search(
    ProductSearchRequest request)
{
    request.Page =
        request.Page < 1
            ? 1
            : request.Page;

    request.PageSize =
        request.PageSize is < 1 or > 50
            ? 12
            : request.PageSize;

    if (request.MinPrice.HasValue &&
        request.MinPrice.Value < 0)
    {
        request.MinPrice = 0;
    }

    if (request.MaxPrice.HasValue &&
        request.MaxPrice.Value < 0)
    {
        request.MaxPrice = 0;
    }

    if (request.MinPrice.HasValue &&
        request.MaxPrice.HasValue &&
        request.MinPrice.Value >
        request.MaxPrice.Value)
    {
        (request.MinPrice, request.MaxPrice) =
            (request.MaxPrice, request.MinPrice);
    }

    var result =
        _repository.Search(request);

    var totalPages =
        (int)Math.Ceiling(
            result.TotalCount /
            (double)request.PageSize);

    if (totalPages > 0 &&
        request.Page > totalPages)
    {
        request.Page = totalPages;

        result =
            _repository.Search(request);
    }

    return new ProductSearchResponseDto
    {
        Products =
            result.Products
                .Select(Map)
                .ToList(),

        TotalCount =
            result.TotalCount,

        Page =
            request.Page,

        PageSize =
            request.PageSize,

        TotalPages =
            totalPages
    };
}

public ProductFilterOptionsDto GetFilterOptions()
{
    return _repository.GetFilterOptions();
}

public ProductResponseDto? GetById(int id)
{
    var product =
        _repository.GetById(id);

    return product == null
        ? null
        : Map(product);
}

public ProductResponseDto Create(
    ProductCreateDto dto)
{
    ValidateProduct(
        dto.Name,
        dto.Description,
        dto.Price,
        dto.StockQuantity);

    var product = new Product
    {
        Name =
            dto.Name.Trim(),

        Description =
            dto.Description.Trim(),

        Category =
            dto.Category?.Trim()
            ?? string.Empty,

        Brand =
            dto.Brand?.Trim()
            ?? string.Empty,

        Price =
            dto.Price,

        StockQuantity =
            dto.StockQuantity,

        ImageUrl =
            dto.ImageUrl?.Trim()
            ?? string.Empty,

        CreatedAt =
            DateTime.UtcNow
    };

    return Map(
        _repository.Create(product));
}

public ProductResponseDto? Update(
    int id,
    ProductUpdateDto dto)
{
    ValidateProduct(
        dto.Name,
        dto.Description,
        dto.Price,
        dto.StockQuantity);

    var existingProduct =
        _repository.GetById(id);

    if (existingProduct == null)
    {
        return null;
    }

    existingProduct.Name =
        dto.Name.Trim();

    existingProduct.Description =
        dto.Description.Trim();

    existingProduct.Category =
        dto.Category?.Trim()
        ?? string.Empty;

    existingProduct.Brand =
        dto.Brand?.Trim()
        ?? string.Empty;

    existingProduct.Price =
        dto.Price;

    existingProduct.StockQuantity =
        dto.StockQuantity;

    existingProduct.ImageUrl =
        dto.ImageUrl?.Trim()
        ?? string.Empty;

    var updatedProduct =
        _repository.Update(
            id,
            existingProduct);

    return updatedProduct == null
        ? null
        : Map(updatedProduct);
}

public bool Delete(int id)
{
    return _repository.Delete(id);
}

public List<ProductResponseDto> ImportFromCsv(
    Stream csvStream)
{
    using var reader =
        new StreamReader(csvStream);

    using var csv =
        new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

    var csvProducts =
        csv.GetRecords<ProductCsvDto>()
            .ToList();

    var importedProducts =
        new List<ProductResponseDto>();

    var skippedProducts = 0;

    foreach (var item in csvProducts)
    {
        if (string.IsNullOrWhiteSpace(item.Name) ||
            item.Price < 0 ||
            item.StockQuantity < 0)
        {
            skippedProducts++;
            continue;
        }

        var product = new Product
        {
            Name =
                item.Name.Trim(),

            Description =
                item.Description?.Trim()
                ?? string.Empty,

            Category =
                item.Category?.Trim()
                ?? string.Empty,

            Brand =
                item.Brand?.Trim()
                ?? string.Empty,

            Price =
                item.Price,

            StockQuantity =
                item.StockQuantity,

            ImageUrl =
                string.IsNullOrWhiteSpace(
                    item.ImageUrl)
                    ? string.Empty
                    : item.ImageUrl.Trim(),

            CreatedAt =
                DateTime.UtcNow
        };

        importedProducts.Add(
            Map(
                _repository.Create(product)
            )
        );
    }

    return importedProducts;
}

private static ProductResponseDto Map(
    Product product)
{
    return new ProductResponseDto
    {
        Id =
            product.Id,

        Name =
            product.Name,

        Description =
            product.Description,

        Category =
            product.Category,

        Brand =
            product.Brand,

        Price =
            product.Price,

        StockQuantity =
            product.StockQuantity,

        ImageUrl =
            product.ImageUrl,

        CreatedAt =
            product.CreatedAt
    };
}

private static void ValidateProduct(
    string name,
    string description,
    decimal price,
    int stockQuantity)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        throw new ArgumentException(
            "Product name is required.");
    }

    if (string.IsNullOrWhiteSpace(description))
    {
        throw new ArgumentException(
            "Product description is required.");
    }

    if (price <= 0)
    {
        throw new ArgumentException(
            "Price must be greater than 0.");
    }

    if (stockQuantity < 0)
    {
        throw new ArgumentException(
            "Stock quantity cannot be negative.");
    }
}

}