namespace ECommerceBackend.DTOs.Product;

public class ProductSearchRequest
{
    public string? Search { get; set; }

    public string? Category { get; set; }

    public string? Brand { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? Stock { get; set; } = "all";

    public string? SortBy { get; set; } = "default";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}
