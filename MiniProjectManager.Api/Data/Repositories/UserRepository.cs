using Microsoft.AspNetCore.Identity;
using MiniProjectManager.Domain.Entities;

namespace MiniProjectManager.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public Task<User> FindByUserNameAsync(string userName)
    {
        return _userManager.FindByNameAsync(userName);
    }

    public Task<User> FindByIdAsync(int id)
    {
        return _userManager.FindByIdAsync(id.ToString());
    }

    public Task<IdentityResult> CreateAsync(User user, string password)
    {
        return _userManager.CreateAsync(user, password);
    }

    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }
}