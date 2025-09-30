using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Domain.Entities;

public class Project
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; }

    public List<TaskItem> Tasks { get; set; } = new();
}