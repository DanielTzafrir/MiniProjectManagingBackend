using MiniProjectManager.Dtos.Task;

namespace MiniProjectManager.Services;

public interface ITaskService
{
    Task<TaskDto> CreateAsync(int projectId, TaskCreateDto dto, int userId);
    Task<TaskDto> UpdateAsync(int taskId, TaskUpdateDto dto, int userId);
    Task DeleteAsync(int taskId, int userId);
}