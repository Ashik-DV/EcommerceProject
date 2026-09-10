using System.Security.Claims;

using ECommerceBackend.DTOs.Order;
using ECommerceBackend.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
private readonly IOrderService _orderService;

public OrderController(IOrderService orderService)
{
    _orderService = orderService;
}

// ======================================================
// GET CURRENT USER ID FROM JWT
// ======================================================

private int GetUserId()
{
    var userIdClaim =
        User.FindFirst(
            ClaimTypes.NameIdentifier
        )?.Value;

    if (!int.TryParse(
            userIdClaim,
            out int userId))
    {
        throw new UnauthorizedAccessException(
            "Invalid user token."
        );
    }

    return userId;
}


// ======================================================
// CHECKOUT
// POST: api/Order/checkout
// ======================================================

[HttpPost("checkout")]
public async Task<IActionResult> Checkout(
    [FromBody] OrderCreateDto dto)
{
    try
    {
        var userId =
            GetUserId();

        var order =
            await _orderService.CreateOrderAsync(
                userId,
                dto
            );

        return Ok(
            order
        );
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(
            new
            {
                message = ex.Message
            }
        );
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(
            new
            {
                message = ex.Message
            }
        );
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An unexpected error occurred."
            }
        );
    }
}


// ======================================================
// VERIFY FAKE PAYMENT
// POST: api/Order/verify-payment
// ======================================================

[HttpPost("verify-payment")]
public async Task<IActionResult> VerifyPayment(
    [FromBody] FakePaymentDto dto)
{
    try
    {
        var userId =
            GetUserId();

        var isVerified =
            await _orderService.VerifyPaymentAsync(
                userId,
                dto
            );

        if (!isVerified)
        {
            return BadRequest(
                new
                {
                    message =
                        "Payment verification failed."
                }
            );
        }

        return Ok(
            new
            {
                message =
                    "Payment successful.",

                orderId =
                    dto.OrderId,

                paymentId =
                    dto.PaymentOrderId,

                status =
                    "Paid"
            }
        );
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(
            new
            {
                message =
                    ex.Message
            }
        );
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(
            new
            {
                message =
                    ex.Message
            }
        );
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An unexpected error occurred."
            }
        );
    }
}


// ======================================================
// GET MY ORDERS
// GET: api/Order
// ======================================================

[HttpGet]
public async Task<IActionResult> GetMyOrders()
{
    try
    {
        var userId =
            GetUserId();

        var orders =
            await _orderService.GetMyOrdersAsync(
                userId
            );

        return Ok(
            orders
        );
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(
            new
            {
                message =
                    ex.Message
            }
        );
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An unexpected error occurred."
            }
        );
    }
}


// ======================================================
// GET MY ORDER BY ID
// GET: api/Order/{id}
// ======================================================

[HttpGet("{id:int}")]
public async Task<IActionResult> GetMyOrderById(
    int id)
{
    try
    {
        var userId =
            GetUserId();

        var order =
            await _orderService.GetMyOrderByIdAsync(
                userId,
                id
            );

        if (order == null)
        {
            return NotFound(
                new
                {
                    message =
                        "Order not found."
                }
            );
        }

        return Ok(
            order
        );
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(
            new
            {
                message =
                    ex.Message
            }
        );
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An unexpected error occurred."
            }
        );
    }
}


// ======================================================
// ADMIN - GET ALL ORDERS
// GET: api/Order/all
// ======================================================

[HttpGet("all")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetAllOrders()
{
    try
    {
        var orders =
            await _orderService.GetAllOrdersAsync();

        return Ok(
            orders
        );
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An unexpected error occurred."
            }
        );
    }
}


// ======================================================
// ADMIN - GET ORDER BY ID
// GET: api/Order/admin/{id}
// ======================================================

[HttpGet("admin/{id:int}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetOrderByIdForAdmin(
    int id)
{
    try
    {
        var order =
            await _orderService.GetOrderByIdForAdminAsync(
                id
            );

        if (order == null)
        {
            return NotFound(
                new
                {
                    message =
                        "Order not found."
                }
            );
        }

        return Ok(
            order
        );
    }
    catch (Exception)
    {
        return StatusCode(
            500,
            new
            {
                message =
                    "An unexpected error occurred."
            }
        );
    }
}

}