using ECommerceBackend.DTOs.Auth;

namespace ECommerceBackend.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto dto);

    Task<LoginResponseDto> LoginAsync(LoginDto dto);

    Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto);
}