using System.Security.Claims;
using ECommerceBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private int GetUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _wishlistService.GetAsync(GetUserId()));
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> Add(int productId)
    {
        try
        {
            return Ok(await _wishlistService.AddAsync(GetUserId(), productId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var removed = await _wishlistService.RemoveAsync(
            GetUserId(),
            productId);

        return removed
            ? NoContent()
            : NotFound(new { message = "Product is not in your wishlist." });
    }
}
