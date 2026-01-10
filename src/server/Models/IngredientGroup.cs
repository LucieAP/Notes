public class IngredientGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Внешние ключи
    public Guid RecipeId { get; set; }

    // Навигационные свойства
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public Recipe Recipe { get; set;}
}