using Moq;
using Xunit;
using MiniProjectManager.Services;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Task;
using AutoMapper;
using System.Threading.Tasks;

namespace MiniProjectManager.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateAsync_AddsTaskToProject()
    {
        var dto = new TaskCreateDto { Title = "Task1" };
        var project = new Project { Id = 1, UserId = 1 };
        var task = new TaskItem { Id = 1, Title = "Task1", ProjectId = 1 };
        _projectRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _mapperMock.Setup(m => m.Map<TaskItem>(dto)).Returns(task);
        _taskRepoMock.Setup(r => r.AddAsync(task)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(new TaskDto { Id = 1, Title = "Task1" });

        var service = new TaskService(_taskRepoMock.Object, _projectRepoMock.Object, _mapperMock.Object);
        var result = await service.CreateAsync(1, dto, 1);

        Assert.Equal(1, result.Id);
        Assert.Equal("Task1", result.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTaskIfOwned()
    {
        var dto = new TaskUpdateDto { Title = "Updated", IsCompleted = true };
        var task = new TaskItem { Id = 1, Title = "Old", ProjectId = 1 };
        var project = new Project { Id = 1, UserId = 1 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _projectRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _mapperMock.Setup(m => m.Map(dto, task));
        _taskRepoMock.Setup(r => r.UpdateAsync(task)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(new TaskDto { Id = 1, Title = "Updated", IsCompleted = true });

        var service = new TaskService(_taskRepoMock.Object, _projectRepoMock.Object, _mapperMock.Object);
        var result = await service.UpdateAsync(1, dto, 1);

        Assert.True(result.IsCompleted);
        Assert.Equal("Updated", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_DeletesTaskIfOwned()
    {
        var task = new TaskItem { Id = 1, ProjectId = 1 };
        var project = new Project { Id = 1, UserId = 1 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _projectRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _taskRepoMock.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);

        var service = new TaskService(_taskRepoMock.Object, _projectRepoMock.Object, _mapperMock.Object);
        await service.DeleteAsync(1, 1);

        _taskRepoMock.Verify(r => r.DeleteAsync(task), Times.Once);
    }
}