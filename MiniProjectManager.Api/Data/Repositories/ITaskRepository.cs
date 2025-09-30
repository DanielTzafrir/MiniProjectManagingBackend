using MiniProjectManager.Domain.Entities;

namespace MiniProjectManager.Data.Repositories;

public interface ITaskRepository
{
    Task<TaskItem> GetByIdAsync(int id);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}