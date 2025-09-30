using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Dtos.Project;
using MiniProjectManager.Services;

namespace MiniProjectManager.Api.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthService _authService;

    public ProjectController(IProjectService projectService, IAuthService authService)
    {
        _projectService = projectService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        var projects = await _projectService.GetAllAsync(userId);
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        var project = await _projectService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        var project = await _projectService.GetByIdAsync(id, userId);
        return Ok(project);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        await _projectService.DeleteAsync(id, userId);
        return NoContent();
    }
}