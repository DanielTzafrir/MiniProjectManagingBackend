namespace MiniProjectManager.Dtos.Project;

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public List<Dtos.Task.TaskDto> Tasks { get; set; } = new();
}