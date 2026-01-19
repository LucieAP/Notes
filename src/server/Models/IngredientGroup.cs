using System.ComponentModel.DataAnnotations;

public class IngredientGroup
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    // Внешние ключи
    [Required(ErrorMessage = "Идентификатор рецепта обязателен для заполнения.")]
    public Guid RecipeId { get; set; }

    // Навигационные свойства
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public Recipe Recipe { get; set;}
}