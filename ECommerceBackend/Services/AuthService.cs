using ECommerceBackend.DTOs.Auth;
using ECommerceBackend.Helpers;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services.Interfaces;
using Google.Apis.Auth;

namespace ECommerceBackend.Services;

public class AuthService : IAuthService
{
private readonly IUserRepository _userRepository;
private readonly PasswordHelper _passwordHelper;
private readonly JwtHelper _jwtHelper;
private readonly IConfiguration _configuration;

public AuthService(
    IUserRepository userRepository,
    PasswordHelper passwordHelper,
    JwtHelper jwtHelper,
    IConfiguration configuration)
{
    _userRepository = userRepository;
    _passwordHelper = passwordHelper;
    _jwtHelper = jwtHelper;
    _configuration = configuration;
}

// ======================================================
// REGISTER
// ======================================================

public async Task<LoginResponseDto> RegisterAsync(RegisterDto dto)
{
    var existingUser =
        await _userRepository.GetByEmailAsync(dto.Email);

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
        await _userRepository.CreateAsync(user);

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

public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
{
    var user =
        await _userRepository.GetByEmailAsync(dto.Email);

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

// ======================================================
// GOOGLE LOGIN
// ======================================================

public async Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.Credential))
    {
        throw new Exception("Google credential is required");
    }

    var clientId = _configuration["Google:ClientId"];

    if (string.IsNullOrWhiteSpace(clientId))
    {
        throw new InvalidOperationException(
            "Google authentication is not configured on the server");
    }

    GoogleJsonWebSignature.Payload payload;

    try
    {
        payload = await GoogleJsonWebSignature.ValidateAsync(
            dto.Credential,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            });
    }
    catch (InvalidJwtException)
    {
        throw new Exception("Invalid Google credential");
    }

    if (string.IsNullOrWhiteSpace(payload.Email) ||
        payload.EmailVerified != true)
    {
        throw new Exception("Google account email is not verified");
    }

    var user = await _userRepository.GetByEmailAsync(payload.Email);

    if (user == null)
    {
        user = await _userRepository.CreateAsync(
            new User
            {
                Name = string.IsNullOrWhiteSpace(payload.Name)
                    ? payload.Email
                    : payload.Name,
                Email = payload.Email,
                PasswordHash = _passwordHelper.HashPassword(
                    Guid.NewGuid().ToString()),
                Role = "User"
            });
    }

    return CreateLoginResponse(user);
}

private LoginResponseDto CreateLoginResponse(User user)
{
    return new LoginResponseDto
    {
        Token = _jwtHelper.GenerateToken(user),
        UserId = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role
    };
}

}