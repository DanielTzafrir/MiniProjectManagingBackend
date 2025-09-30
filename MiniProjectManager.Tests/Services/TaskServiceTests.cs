using AutoMapper;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Task;
using MiniProjectManager.Services;
using Moq;
using Xunit;

namespace MiniProjectManager.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _service = new TaskService(_taskRepoMock.Object, _projectRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_PartialToggle_UpdatesOnlyIsCompleted()
    {
        var dto = new TaskUpdateDto { IsCompleted = true };  // Partial, no title/dueDate
        var task = new TaskItem { Id = 1, Title = "Test", IsCompleted = false, ProjectId = 1 };
        var project = new Project { Id = 1, UserId = 1 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _projectRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(new TaskDto { Id = 1, IsCompleted = true });

        var result = await _service.UpdateAsync(1, dto, 1);

        Assert.True(result.IsCompleted);
        _taskRepoMock.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t => t.Title == "Test" && t.IsCompleted == true)), Times.Once);  // Title unchanged
    }

    [Fact]
    public async Task UpdateAsync_NotOwned_ThrowsUnauthorized()
    {
        var dto = new TaskUpdateDto { IsCompleted = true };
        var task = new TaskItem { Id = 1, ProjectId = 1 };
        var project = new Project { Id = 1, UserId = 2 };  // Different user
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _projectRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateAsync(1, dto, 1));
    }
}