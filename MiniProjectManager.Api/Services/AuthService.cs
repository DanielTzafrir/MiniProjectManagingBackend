using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Auth;
using Microsoft.Extensions.Configuration;

namespace MiniProjectManager.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration config)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _config = config;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
{
    if (string.IsNullOrEmpty(dto.username) || string.IsNullOrEmpty(dto.email) || string.IsNullOrEmpty(dto.password))
    {
        throw new ArgumentException("Invalid registration data");
    }

    var user = _mapper.Map<User>(dto);
    var result = await _userRepository.CreateAsync(user, dto.password);
    if (!result.Succeeded)
    {
        throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    return GenerateJwtToken(user);
}

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.FindByUserNameAsync(dto.UserName);
        if (user == null || !await _userRepository.CheckPasswordAsync(user, dto.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        return GenerateJwtToken(user);
    }

    public int GetUserIdFromClaims(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user claim");
        }
        return userId;
    }

    private string GenerateJwtToken(User user)
    {
        var keyString = _configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(keyString) || Encoding.UTF8.GetBytes(keyString).Length < 16)
        {
            throw new InvalidOperationException("JWT key is invalid or too short");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: _configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}