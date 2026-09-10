using System.Security.Claims;

using ECommerceBackend.DTOs.Cart;
using ECommerceBackend.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
private readonly ICartService _cartService;

public CartController(ICartService cartService)
{
    _cartService = cartService;
}

// ======================================================
// GET CART
// GET: /api/Cart
// ======================================================

[HttpGet]
public IActionResult GetCart()
{
    try
    {
        var userId = GetUserId();

        var cart = _cartService.GetCart(userId);

        return Ok(cart);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new
        {
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

// ======================================================
// ADD TO CART
// POST: /api/Cart
// ======================================================

[HttpPost]
public IActionResult AddToCart(
    [FromBody] AddToCartDto dto)
{
    try
    {
        var userId = GetUserId();

        var cart = _cartService.AddToCart(
            userId,
            dto
        );

        return Ok(cart);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new
        {
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

// ======================================================
// UPDATE CART ITEM
// PUT: /api/Cart/{cartItemId}
// ======================================================

[HttpPut("{cartItemId}")]
public IActionResult UpdateCartItem(
    int cartItemId,
    [FromBody] UpdateCartItemDto dto)
{
    try
    {
        var userId = GetUserId();

        var cart = _cartService.UpdateCartItem(
            userId,
            cartItemId,
            dto
        );

        return Ok(cart);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new
        {
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

// ======================================================
// REMOVE CART ITEM
// DELETE: /api/Cart/{cartItemId}
// ======================================================

[HttpDelete("{cartItemId}")]
public IActionResult RemoveCartItem(
    int cartItemId)
{
    try
    {
        var userId = GetUserId();

        var removed = _cartService.RemoveCartItem(
            userId,
            cartItemId
        );

        if (!removed)
        {
            return NotFound(new
            {
                message = "Cart item not found."
            });
        }

        return Ok(new
        {
            message = "Cart item removed successfully."
        });
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new
        {
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

// ======================================================
// CLEAR CART
// DELETE: /api/Cart
// ======================================================

[HttpDelete]
public IActionResult ClearCart()
{
    try
    {
        var userId = GetUserId();

        var cleared = _cartService.ClearCart(userId);

        return Ok(new
        {
            message = "Cart cleared successfully.",
            success = cleared
        });
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new
        {
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

// ======================================================
// GET USER ID FROM JWT
// ======================================================

private int GetUserId()
{
    var userIdClaim = User.FindFirst(
        ClaimTypes.NameIdentifier
    );

    if (userIdClaim == null)
    {
        throw new UnauthorizedAccessException(
            "User ID was not found in the JWT token."
        );
    }

    if (!int.TryParse(
            userIdClaim.Value,
            out var userId))
    {
        throw new UnauthorizedAccessException(
            "Invalid User ID in JWT token."
        );
    }

    return userId;
}

}