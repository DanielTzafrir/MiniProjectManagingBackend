using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Dtos.Task;

public class TaskCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; }

    public DateTime? DueDate { get; set; }
}