using ECommerceBackend.DTOs.Auth;

namespace ECommerceBackend.Services.Interfaces;

public interface IAuthService
{
    LoginResponseDto Register(RegisterDto dto);

    LoginResponseDto Login(LoginDto dto);
}