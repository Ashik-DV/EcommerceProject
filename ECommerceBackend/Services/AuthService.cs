using ECommerceBackend.DTOs.Auth;
using ECommerceBackend.Helpers;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services.Interfaces;

namespace ECommerceBackend.Services;

public class AuthService : IAuthService
{
private readonly IUserRepository _userRepository;
private readonly PasswordHelper _passwordHelper;
private readonly JwtHelper _jwtHelper;

public AuthService(
    IUserRepository userRepository,
    PasswordHelper passwordHelper,
    JwtHelper jwtHelper)
{
    _userRepository = userRepository;
    _passwordHelper = passwordHelper;
    _jwtHelper = jwtHelper;
}

// ======================================================
// REGISTER
// ======================================================

public LoginResponseDto Register(RegisterDto dto)
{
    var existingUser =
        _userRepository.GetByEmail(dto.Email);

    if (existingUser != null)
    {
        throw new Exception("Email already exists");
    }

    var user = new User
    {
        Name = dto.Name,
        Email = dto.Email,
        PasswordHash =
            _passwordHelper.HashPassword(dto.Password),
        Role = "User"
    };

    var createdUser =
        _userRepository.Create(user);

    var token =
        _jwtHelper.GenerateToken(createdUser);

    return new LoginResponseDto
    {
        Token = token,
        UserId = createdUser.Id,
        Name = createdUser.Name,
        Email = createdUser.Email,
        Role = createdUser.Role
    };
}

// ======================================================
// LOGIN
// ======================================================

public LoginResponseDto Login(LoginDto dto)
{
    var user =
        _userRepository.GetByEmail(dto.Email);

    if (user == null)
    {
        throw new Exception(
            "Invalid email or password");
    }

    var passwordValid =
        _passwordHelper.VerifyPassword(
            dto.Password,
            user.PasswordHash);

    if (!passwordValid)
    {
        throw new Exception(
            "Invalid email or password");
    }

    var token =
        _jwtHelper.GenerateToken(user);

    return new LoginResponseDto
    {
        Token = token,
        UserId = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role
    };
}

}