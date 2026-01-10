public class Ingredient 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; } 
    public float Quantity { get; set; } 
    public Unit Unit { get; set; } = Unit.Default;
    public string? Note { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Внешние ключи
    public Guid? IngredientGroupId { get; set; } 
    public Guid RecipeId { get; set; }

    // Навигационные свойства
    public IngredientGroup? IngredientGroup { get; set; }
    public Recipe Recipe { get; set; }
}
