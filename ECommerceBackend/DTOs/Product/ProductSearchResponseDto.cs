namespace ECommerceBackend.DTOs.Product;

public class ProductSearchResponseDto
{
    public List<ProductResponseDto> Products { get; set; } = [];

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}
