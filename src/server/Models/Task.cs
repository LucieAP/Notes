public class Task
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool CheckBox {get; set; }
    public bool IsFavorite{ get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public BackgroundColor BackgroundColor { get; set; } = BackgroundColor.Default;
    public bool IsDeleted { get; set; } 
    public DateTime DeletedAt { get; set; }

    // Внешние ключи 
    public Guid CreatedBy { get; set; } 
    public Guid? TaskGroupId  {get; set; }

    // Навигационные свойства
    public User User { get; set; }
    public TaskGroup? TaskGroup { get; set; }
}