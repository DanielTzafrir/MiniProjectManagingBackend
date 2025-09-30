using AutoMapper;
using MiniProjectManager.Data.Repositories;
using MiniProjectManager.Domain.Entities;
using MiniProjectManager.Dtos.Project;

namespace MiniProjectManager.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProjectService(IProjectRepository repository, IUserRepository userRepository, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<ProjectDto>> GetAllAsync(int userId)
    {
        var projects = await _repository.GetAllByUserIdAsync(userId);
        return _mapper.Map<List<ProjectDto>>(projects);
    }

    public async Task<ProjectDto> GetByIdAsync(int id, int userId)
    {
        var project = await GetProjectIfOwned(id, userId);
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> CreateAsync(ProjectCreateDto dto, int userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var project = _mapper.Map<Project>(dto);
        project.UserId = userId;
        await _repository.AddAsync(project);
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var project = await GetProjectIfOwned(id, userId);
        await _repository.DeleteAsync(project);
    }

    private async Task<Project> GetProjectIfOwned(int id, int userId)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project == null || project.UserId != userId)
        {
            throw new KeyNotFoundException("Project not found or not owned");
        }
        return project;
    }
}