public class RecipeStep
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Внешние ключи
    public Guid RecipeId { get; set; }

    // Навигационные свойства
    public Recipe Recipe { get; set;}
}