using MiniProjectManager.Domain.Entities;

namespace MiniProjectManager.Data.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllByUserIdAsync(int userId);
    Task<Project> GetByIdAsync(int id);
    Task AddAsync(Project project);
    Task DeleteAsync(Project project);
}