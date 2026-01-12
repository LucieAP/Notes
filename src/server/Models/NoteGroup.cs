using System.ComponentModel.DataAnnotations.Schema;

public class NoteGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } 
    public bool isDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } 

    // Внешние ключи 
    [ForeignKey("User")]
    public Guid CreatedBy { get; set; } 

    // Навигационные свойства
    public User User { get; set; }
    public ICollection<Note> Notes {get; set; } = new List<Note>(); 
}