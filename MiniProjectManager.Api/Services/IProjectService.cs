using MiniProjectManager.Dtos.Project;

namespace MiniProjectManager.Services;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync(int userId);
    Task<ProjectDto> GetByIdAsync(int id, int userId);
    Task<ProjectDto> CreateAsync(ProjectCreateDto dto, int userId);
    Task DeleteAsync(int id, int userId);
}