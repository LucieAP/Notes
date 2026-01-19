using System.ComponentModel.DataAnnotations;

public class User 
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "GoogleId обязателен для заполнения.")]
    [StringLength(100, ErrorMessage = "GoogleId не может превышать 100 символов.")]
    public required string GoogleId { get; set; } 

    [Required(ErrorMessage = "Имя обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Имя должно содержать от 1 до 100 символов.")]
    public required string Name { get; set; } 

    [Required(ErrorMessage = "Email обязателен для заполнения.")]
    [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты.")]
    [StringLength(254, ErrorMessage = "Email не может превышать 254 символа.")] 
    public required string Email { get; set; } 
    public bool EmailVerified { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; } 

    [Url(ErrorMessage = "Некорректный URL для изображения.")]
    public string? Picture { get; set; }

    // Навигационные свойства
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<TaskGroup> TaskGroups { get; set; } = new List<TaskGroup>();
    public ICollection<NoteGroup> NoteGroups { get; set; } = new List<NoteGroup>();

}