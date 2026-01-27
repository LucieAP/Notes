using System.ComponentModel.DataAnnotations;

public record CreateRecipeStepRequest
{
    [Required(ErrorMessage = "Описание обязательно для заполнения.")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Описание должно содержать от 1 до 1000 символов.")]
    public required string Description { get; set; }
    public Guid RecipeId { get; set; }
}
