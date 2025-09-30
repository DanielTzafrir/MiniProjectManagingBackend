using AutoMapper;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Task;

namespace MiniProjectManager.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<TaskDto> CreateAsync(int projectId, TaskCreateDto dto, int userId)
    {
        var project = await GetProjectIfOwned(projectId, userId);
        var task = _mapper.Map<TaskItem>(dto);
        task.ProjectId = projectId;
        await _taskRepository.AddAsync(task);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> UpdateAsync(int taskId, TaskUpdateDto dto, int userId)
    {
        var task = await GetTaskIfOwned(taskId, userId);
        _mapper.Map(dto, task);
        await _taskRepository.UpdateAsync(task);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task DeleteAsync(int taskId, int userId)
    {
        var task = await GetTaskIfOwned(taskId, userId);
        await _taskRepository.DeleteAsync(task);
    }

    private async Task<Project> GetProjectIfOwned(int projectId, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId)
        {
            throw new KeyNotFoundException("Project not found or not owned");
        }
        return project;
    }

    private async Task<TaskItem> GetTaskIfOwned(int taskId, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            throw new KeyNotFoundException("Task not found");
        }
        var project = await GetProjectIfOwned(task.ProjectId, userId);
        return task;
    }
}