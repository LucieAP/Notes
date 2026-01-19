using System.ComponentModel.DataAnnotations;

public class RecipeStep
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Описание обязательно для заполнения.")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Описание должно содержать от 1 до 1000 символов.")]
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Внешние ключи
    [Required(ErrorMessage = "Идентификатор рецепта обязателен для заполнения.")]
    public Guid RecipeId { get; set; }

    // Навигационные свойства
    public Recipe Recipe { get; set;}
}