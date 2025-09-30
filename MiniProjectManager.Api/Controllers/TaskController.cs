using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Dtos.Task;
using MiniProjectManager.Services;
namespace MiniProjectManager.Api.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IAuthService _authService;

    public TaskController(ITaskService taskService, IAuthService authService)
    {
        _taskService = taskService;
        _authService = authService;
    }

    [HttpPost("{projectId}/tasks")]
    public async Task<IActionResult> Create(int projectId, [FromBody] TaskCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        var task = await _taskService.CreateAsync(projectId, dto, userId);
        return Created($"/api/tasks/{task.Id}", task);
    }

    [HttpPut("tasks/{taskId}")]
    public async Task<IActionResult> Update(int taskId, [FromBody] TaskUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        var task = await _taskService.UpdateAsync(taskId, dto, userId);
        return Ok(task);
    }

    [HttpDelete("tasks/{taskId}")]
    public async Task<IActionResult> Delete(int taskId)
    {
        var userId = _authService.GetUserIdFromClaims(HttpContext);
        await _taskService.DeleteAsync(taskId, userId);
        return NoContent();
    }
}