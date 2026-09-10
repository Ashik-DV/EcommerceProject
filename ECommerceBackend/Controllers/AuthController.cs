using ECommerceBackend.DTOs.Auth;
using ECommerceBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
private readonly IAuthService _authService;

public AuthController(IAuthService authService)
{
    _authService = authService;
}

// ======================================================
// REGISTER
// POST: /api/Auth/register
// ======================================================

[HttpPost("register")]
public IActionResult Register(RegisterDto dto)
{
    try
    {
        var result = _authService.Register(dto);

        return Ok(result);
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
// LOGIN
// POST: /api/Auth/login
// ======================================================

[HttpPost("login")]
public IActionResult Login(LoginDto dto)
{
    try
    {
        var result = _authService.Login(dto);
        

        return Ok(result);
    }
    catch (Exception ex)
    {
        return Unauthorized(new
        {
            message = ex.Message
        });
    }
}

}