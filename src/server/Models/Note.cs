using System.ComponentModel.DataAnnotations.Schema;

public class Note
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public BackgroundColor BackgroundColor { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime? DeletedAt { get; set; }

    // Внешние ключи 
    [ForeignKey("User")]
    public Guid CreatedBy { get; set; } 
    public Guid? NoteGroupId  {get; set;}

    // Навигационные свойства
    public User User { get; set; }
    public NoteGroup? NoteGroup { get; set; }
}