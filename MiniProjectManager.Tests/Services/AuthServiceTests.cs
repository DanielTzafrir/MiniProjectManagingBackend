using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Auth;
using MiniProjectManager.Services;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MiniProjectManager.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IConfiguration> _configMock = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _configMock.Setup(c => c["Jwt:Key"]).Returns("super-secret-key-with-256-bits-length");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");
        _service = new AuthService(_userRepoMock.Object, _mapperMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_SuccessfulCreation_ReturnsToken()
    {
        var dto = new RegisterDto { UserName = "test", Email = "test@example.com", Password = "Pass123" };
        var user = new User { UserName = "test", Email = "test@example.com" };
        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
        _userRepoMock.Setup(r => r.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);

        var token = await _service.RegisterAsync(dto);

        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task RegisterAsync_FailedCreation_ThrowsException()
    {
        var dto = new RegisterDto { UserName = "test", Email = "test@example.com", Password = "Pass123" };
        var user = new User { UserName = "test" };
        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
        _userRepoMock.Setup(r => r.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var dto = new LoginDto { UserName = "test", Password = "Pass123" };
        var user = new User { Id = 1, UserName = "test" };
        _userRepoMock.Setup(r => r.FindByUserNameAsync(dto.UserName)).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);

        var token = await _service.LoginAsync(dto);

        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorized()
    {
        var dto = new LoginDto { UserName = "test", Password = "Wrong" };
        _userRepoMock.Setup(r => r.FindByUserNameAsync(dto.UserName)).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(dto));
    }

    [Fact]
    public void GetUserIdFromClaims_ValidClaim_ReturnsId()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }));

        var userId = _service.GetUserIdFromClaims(httpContext);

        Assert.Equal(1, userId);
    }

    [Fact]
    public void GetUserIdFromClaims_InvalidClaim_ThrowsUnauthorized()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Throws<UnauthorizedAccessException>(() => _service.GetUserIdFromClaims(httpContext));
    }
}