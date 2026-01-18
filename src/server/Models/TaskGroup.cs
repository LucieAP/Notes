using System.ComponentModel.DataAnnotations.Schema;

public class TaskGroup
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; } 
    public bool isDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } 

    // Внешние ключи 
    [ForeignKey("User")]
    public Guid CreatedBy { get; set; } 

    // Навигационные свойства
    public User User { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}