using ECommerceBackend.DTOs.Product;
using ECommerceBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
private readonly IProductService _productService;

public ProductsController(IProductService productService)
{
    _productService = productService;
}

[HttpGet]
[Authorize]
public IActionResult GetProducts(
    [FromQuery] ProductSearchRequest request)
{
    try
    {
        return Ok(_productService.Search(request));
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An error occurred while fetching products."
            });
    }
}

[HttpGet("filters")]
[Authorize]
public IActionResult GetFilterOptions()
{
    try
    {
        return Ok(_productService.GetFilterOptions());
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An error occurred while fetching filter options."
            });
    }
}

[HttpGet("{id:int}")]
[Authorize]
public IActionResult GetProduct(int id)
{
    try
    {
        var product = _productService.GetById(id);

        return product == null
            ? NotFound(new
            {
                message = "Product not found."
            })
            : Ok(product);
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An error occurred while fetching the product."
            });
    }
}

[HttpPost]
[Authorize(Roles = "Admin")]
public IActionResult CreateProduct(
    ProductCreateDto dto)
{
    try
    {
        var createdProduct =
            _productService.Create(dto);

        return CreatedAtAction(
            nameof(GetProduct),
            new
            {
                id = createdProduct.Id
            },
            createdProduct);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An error occurred while creating the product."
            });
    }
}

[HttpPut("{id:int}")]
[Authorize(Roles = "Admin")]
public IActionResult UpdateProduct(
    int id,
    ProductUpdateDto dto)
{
    try
    {
        var updatedProduct =
            _productService.Update(id, dto);

        return updatedProduct == null
            ? NotFound(new
            {
                message = "Product not found."
            })
            : Ok(updatedProduct);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An error occurred while updating the product."
            });
    }
}

[HttpDelete("{id:int}")]
[Authorize(Roles = "Admin")]
public IActionResult DeleteProduct(int id)
{
    try
    {
        return _productService.Delete(id)
            ? NoContent()
            : NotFound(new
            {
                message = "Product not found."
            });
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An error occurred while deleting the product."
            });
    }
}

[HttpPost("import-csv")]
[Authorize(Roles = "Admin")]
public IActionResult ImportCsv(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        return BadRequest(new
        {
            message = "Please select a CSV file."
        });
    }

    if (!file.FileName.EndsWith(
            ".csv",
            StringComparison.OrdinalIgnoreCase))
    {
        return BadRequest(new
        {
            message = "Only CSV files are allowed."
        });
    }

    try
    {
        using var stream = file.OpenReadStream();

        var products =
            _productService.ImportFromCsv(stream);

        return Ok(new
        {
            message =
                $"{products.Count} products imported successfully.",

            products
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = "Failed to import CSV file.",
            error = ex.Message
        });
    }
}

}