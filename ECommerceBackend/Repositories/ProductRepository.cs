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

public async Task<List<Product>> GetAllAsync()
{
    var products = _context.Products
        .AsNoTracking()
        .OrderByDescending(p => p.Id)
        .ToListAsync();

    return await products;
}

public async Task<ProductSearchResult> SearchAsync(
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

    var totalCount = await query.CountAsync();

    var page =
        request.Page < 1
            ? 1
            : request.Page;

    var pageSize =
        request.PageSize is < 1 or > 50
            ? 12
            : request.PageSize;

    var products = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new ProductSearchResult
    {
        Products = products,
        TotalCount = totalCount
    };
}

public async Task<ProductFilterOptionsDto> GetFilterOptionsAsync()
{
    var categories = await _context.Products
        .AsNoTracking()
        .Where(p => p.Category != "")
        .Select(p => p.Category)
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync();

    var brands = await _context.Products
        .AsNoTracking()
        .Where(p => p.Brand != "")
        .Select(p => p.Brand)
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync();

    return new ProductFilterOptionsDto
    {
        Categories = categories,
        Brands = brands
    };
}

public async Task<Product?> GetByIdAsync(int id)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == id);

    return product;
}

public async Task<Product> CreateAsync(Product product)
{
    _context.Products.Add(product);

    await _context.SaveChangesAsync();

    return product;
}

public async Task<Product?> UpdateAsync(
    int id,
    Product product)
{
    var existingProduct =
        await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

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

    await _context.SaveChangesAsync();

    return existingProduct;
}

public async Task<bool> DeleteAsync(int id)
{
    var product =
        await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
        return false;
    }

    var cartItems =
        await _context.CartItems
            .Where(ci => ci.ProductId == id)
            .ToListAsync();

    if (cartItems.Count > 0)
    {
        _context.CartItems.RemoveRange(cartItems);
    }

    var orderItems =
        await _context.OrderItems
            .Where(oi => oi.ProductId == id)
            .ToListAsync();

    if (orderItems.Count > 0)
    {
        _context.OrderItems.RemoveRange(orderItems);
    }

    _context.Products.Remove(product);

    await _context.SaveChangesAsync();

    return true;
}

}