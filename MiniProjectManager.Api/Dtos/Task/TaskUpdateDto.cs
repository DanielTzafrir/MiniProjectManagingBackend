using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Dtos.Task;

public class TaskUpdateDto
{
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
}