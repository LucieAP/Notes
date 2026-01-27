using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using server.Interfaces;

public class Task : IUpdatableItem, ISoftDeletable, IUpdatableItemColor
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Заголовок обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public required string Title { get; set; }
    [StringLength(5000, ErrorMessage = "Описание не может превышать 5000 символов.")]
    public string? Description { get; set; }
    public bool CheckBox {get; set; }
    public bool IsFavorite{ get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    [Required(ErrorMessage = "Цвет фона обязателен для заполнения.")]
    public BackgroundColor BackgroundColor { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime? DeletedAt { get; set; }

    // Внешние ключи 
    [ForeignKey("User")]
    [Required(ErrorMessage = "Идентификатор создателя обязателен для заполнения.")]
    public Guid CreatedBy { get; set; } 
    public Guid? TaskGroupId  {get; set; }

    // Навигационные свойства
    public User User { get; set; }
    public TaskGroup? TaskGroup { get; set; }
}