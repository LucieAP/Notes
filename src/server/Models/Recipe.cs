using System.ComponentModel.DataAnnotations.Schema;

public class Recipe
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime DeletedAt { get; set; }

    // Внешние ключи 
    [ForeignKey("User")]
    public Guid CreatedBy { get; set; }

    // Навигационные свойства
    public User User { get; set; }
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<IngredientGroup> IngredientGroups { get; set; } = new List<IngredientGroup>();
    public ICollection<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();
}