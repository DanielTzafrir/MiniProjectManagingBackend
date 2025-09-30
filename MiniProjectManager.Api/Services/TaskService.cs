using AutoMapper;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Task;

namespace MiniProjectManager.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository repository, IProjectRepository projectRepository, IMapper mapper)
    {
        _repository = repository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<TaskDto> GetByIdAsync(int id, int userId)
    {
        var task = await GetTaskIfOwned(id, userId);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateAsync(int projectId, TaskCreateDto dto, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId)
        {
            throw new KeyNotFoundException("Project not found or not owned");
        }

        var task = _mapper.Map<TaskItem>(dto);
        task.ProjectId = projectId;
        await _repository.AddAsync(task);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> UpdateAsync(int id, TaskUpdateDto dto, int userId)
    {
        var task = await GetTaskIfOwned(id, userId);

        if (dto.Title != null)
        {
            task.Title = dto.Title;
        }
        if (dto.DueDate != null)
        {
            task.DueDate = dto.DueDate;
        }
        task.IsCompleted = dto.IsCompleted;  

        await _repository.UpdateAsync(task);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var task = await GetTaskIfOwned(id, userId);
        await _repository.DeleteAsync(task);
    }

    private async Task<TaskItem> GetTaskIfOwned(int id, int userId)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null)
        {
            throw new KeyNotFoundException("Task not found");
        }

        var project = await _projectRepository.GetByIdAsync(task.ProjectId);
        if (project == null || project.UserId != userId)
        {
            throw new UnauthorizedAccessException("Task not owned");
        }

        return task;
    }
}