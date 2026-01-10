public class User 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string GoogleId { get; set; } 
    public required string Name { get; set; } 
    public required string Email { get; set; } 
    public bool EmailVerified { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } 
    public string? Picture { get; set; }

    // Навигационные свойства
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<TaskGroup> TaskGroups { get; set; } = new List<TaskGroup>();
    public ICollection<NoteGroup> NoteGroups { get; set; } = new List<NoteGroup>();

}