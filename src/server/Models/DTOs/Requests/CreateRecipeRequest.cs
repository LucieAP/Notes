using System.ComponentModel.DataAnnotations;

public record CreateRecipeRequest
{
    [Required(ErrorMessage = "Заголовок обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public required string Title { get; set; }
    [StringLength(5000, ErrorMessage = "Описание не может превышать 5000 символов.")]
    public string? Description { get; set; }
    public bool IsFavorite { get; set; }
    public Guid? RecipeGroupId { get; set; }
}