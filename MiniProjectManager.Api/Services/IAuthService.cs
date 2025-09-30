using MiniProjectManager.Dtos.Auth;

namespace MiniProjectManager.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    int GetUserIdFromClaims(HttpContext httpContext);
}