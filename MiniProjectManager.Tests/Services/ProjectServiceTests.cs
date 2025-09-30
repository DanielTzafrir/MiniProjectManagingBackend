using Moq;
using Xunit;
using MiniProjectManager.Services;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Project;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjectManager.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateAsync_AddsProjectForUser()
    {
        var dto = new ProjectCreateDto { Title = "Proj" };
        var user = new User { Id = 1 };
        var project = new Project { Id = 1, Title = "Proj", UserId = 1 };
        _userRepoMock.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<Project>(dto)).Returns(project);
        _projectRepoMock.Setup(r => r.AddAsync(project)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ProjectDto>(project)).Returns(new ProjectDto { Id = 1, Title = "Proj" });

        var service = new ProjectService(_projectRepoMock.Object, _userRepoMock.Object, _mapperMock.Object);
        var result = await service.CreateAsync(dto, 1);

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsUserProjects()
    {
        var projects = new List<Project> { new Project { Id = 1, UserId = 1 } };
        _projectRepoMock.Setup(r => r.GetAllByUserIdAsync(1)).ReturnsAsync(projects);
        _mapperMock.Setup(m => m.Map<List<ProjectDto>>(projects)).Returns(new List<ProjectDto> { new ProjectDto { Id = 1 } });

        var service = new ProjectService(_projectRepoMock.Object, _userRepoMock.Object, _mapperMock.Object);
        var result = await service.GetAllAsync(1);

        Assert.Single(result);
    }
}