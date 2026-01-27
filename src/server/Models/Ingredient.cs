using System.ComponentModel.DataAnnotations;

public class Ingredient
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Название обязательно для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Название должно содержать от 1 до 100 символов.")]
    public required string Name { get; set; } 

    [Range(0.001, float.MaxValue, ErrorMessage = "Количество должно быть положительным числом.")]
    public float Quantity { get; set; } 

    [Required(ErrorMessage = "Единица измерения обязательна для заполнения.")]
    public Unit Unit { get; set; }

    [StringLength(500, ErrorMessage = "Примечание не может превышать 500 символов.")]
    public string? Note { get; set; } 
    public DateTime CreatedAt { get; set; }

    // Внешние ключи
    public Guid? IngredientGroupId { get; set; } 

    [Required(ErrorMessage = "Идентификатор рецепта обязателен для заполнения.")]
    public Guid RecipeId { get; set; }

    // Навигационные свойства
    public IngredientGroup? IngredientGroup { get; set; }
    public Recipe Recipe { get; set; }
}