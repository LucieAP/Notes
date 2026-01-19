using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskGroup
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; } 
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } 

    // Внешние ключи 
    [ForeignKey("User")]
    [Required(ErrorMessage = "Идентификатор создателя обязателен для заполнения.")]
    public Guid CreatedBy { get; set; } 

    // Навигационные свойства
    public User User { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}