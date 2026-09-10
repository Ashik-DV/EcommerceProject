using CsvHelper.Configuration.Attributes;

namespace ECommerceBackend.DTOs.Product;

public class ProductCsvDto
{
    [Name("Product Name")]
    public string Name { get; set; } = string.Empty;

    [Name("Description")]
    public string Description { get; set; } = string.Empty;

    [Name("Category")]
    [Optional]
    public string Category { get; set; } = string.Empty;

    [Name("Brand")]
    [Optional]
    public string Brand { get; set; } = string.Empty;

    [Name("Price")]
    public decimal Price { get; set; }

    [Name("Stock Quantity")]
    public int StockQuantity { get; set; }

    [Name("Image URL")]
    public string? ImageUrl { get; set; }
}
