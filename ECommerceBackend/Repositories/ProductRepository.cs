using ECommerceBackend.Data;
using ECommerceBackend.DTOs.Product;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Repositories;

public class ProductSearchResult
{
public List<Product> Products { get; set; } = [];
public int TotalCount { get; set; }
}

public class ProductRepository : IProductRepository
{
private readonly AppDbContext _context;

public ProductRepository(AppDbContext context)
{
    _context = context;
}

public List<Product> GetAll()
{
    var products = _context.Products
        .AsNoTracking()
        .OrderByDescending(p => p.Id)
        .ToList();

    return products;
}

public ProductSearchResult Search(
    ProductSearchRequest request)
{
    var query = _context.Products
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(request.Search))
    {
        var search = request.Search.Trim();
        var pattern = $"%{search}%";

        query = query.Where(p =>
            EF.Functions.Like(p.Name, pattern) ||
            EF.Functions.Like(p.Description, pattern) ||
            EF.Functions.Like(p.Category, pattern) ||
            EF.Functions.Like(p.Brand, pattern));
    }

    if (!string.IsNullOrWhiteSpace(request.Category) &&
        !request.Category.Equals(
            "all",
            StringComparison.OrdinalIgnoreCase))
    {
        var category = request.Category.Trim();

        query = query.Where(
            p => p.Category == category);
    }

    if (!string.IsNullOrWhiteSpace(request.Brand) &&
        !request.Brand.Equals(
            "all",
            StringComparison.OrdinalIgnoreCase))
    {
        var brand = request.Brand.Trim();

        query = query.Where(
            p => p.Brand == brand);
    }

    if (request.MinPrice.HasValue)
    {
        query = query.Where(
            p => p.Price >= request.MinPrice.Value);
    }

    if (request.MaxPrice.HasValue)
    {
        query = query.Where(
            p => p.Price <= request.MaxPrice.Value);
    }

    if (request.Stock?.Equals(
            "in-stock",
            StringComparison.OrdinalIgnoreCase) == true)
    {
        query = query.Where(
            p => p.StockQuantity > 0);
    }
    else if (request.Stock?.Equals(
                 "out-of-stock",
                 StringComparison.OrdinalIgnoreCase) == true)
    {
        query = query.Where(
            p => p.StockQuantity <= 0);
    }

    query = request.SortBy?.ToLowerInvariant() switch
    {
        "price-low" =>
            query
                .OrderBy(p => p.Price)
                .ThenBy(p => p.Id),

        "price-high" =>
            query
                .OrderByDescending(p => p.Price)
                .ThenByDescending(p => p.Id),

        "name-az" =>
            query
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id),

        "name-za" =>
            query
                .OrderByDescending(p => p.Name)
                .ThenByDescending(p => p.Id),

        _ =>
            query.OrderByDescending(p => p.Id)
    };

    var totalCount = query.Count();

    var page =
        request.Page < 1
            ? 1
            : request.Page;

    var pageSize =
        request.PageSize is < 1 or > 50
            ? 12
            : request.PageSize;

    var products = query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return new ProductSearchResult
    {
        Products = products,
        TotalCount = totalCount
    };
}

public ProductFilterOptionsDto GetFilterOptions()
{
    var categories = _context.Products
        .AsNoTracking()
        .Where(p => p.Category != "")
        .Select(p => p.Category)
        .Distinct()
        .OrderBy(x => x)
        .ToList();

    var brands = _context.Products
        .AsNoTracking()
        .Where(p => p.Brand != "")
        .Select(p => p.Brand)
        .Distinct()
        .OrderBy(x => x)
        .ToList();

    return new ProductFilterOptionsDto
    {
        Categories = categories,
        Brands = brands
    };
}

public Product? GetById(int id)
{
    var product = _context.Products
        .FirstOrDefault(p => p.Id == id);

    return product;
}

public Product Create(Product product)
{
    _context.Products.Add(product);

    _context.SaveChanges();

    return product;
}

public Product? Update(
    int id,
    Product product)
{
    var existingProduct =
        _context.Products
            .FirstOrDefault(p => p.Id == id);

    if (existingProduct == null)
    {
        return null;
    }

    existingProduct.Name = product.Name;
    existingProduct.Description = product.Description;
    existingProduct.Category = product.Category;
    existingProduct.Brand = product.Brand;
    existingProduct.Price = product.Price;
    existingProduct.StockQuantity = product.StockQuantity;
    existingProduct.ImageUrl = product.ImageUrl;

    _context.SaveChanges();

    return existingProduct;
}

public bool Delete(int id)
{
    var product =
        _context.Products
            .FirstOrDefault(p => p.Id == id);

    if (product == null)
    {
        return false;
    }

    var cartItems =
        _context.CartItems
            .Where(ci => ci.ProductId == id)
            .ToList();

    if (cartItems.Count > 0)
    {
        _context.CartItems.RemoveRange(cartItems);
    }

    var orderItems =
        _context.OrderItems
            .Where(oi => oi.ProductId == id)
            .ToList();

    if (orderItems.Count > 0)
    {
        _context.OrderItems.RemoveRange(orderItems);
    }

    _context.Products.Remove(product);

    _context.SaveChanges();

    return true;
}

}