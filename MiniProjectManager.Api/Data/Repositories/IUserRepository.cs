using Microsoft.AspNetCore.Identity;
using MiniProjectManager.Domain.Entities;

namespace MiniProjectManager.Data.Repositories;

public interface IUserRepository
{
    Task<User> FindByUserNameAsync(string userName);
    Task<User> FindByIdAsync(int id);
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<bool> CheckPasswordAsync(User user, string password);
}