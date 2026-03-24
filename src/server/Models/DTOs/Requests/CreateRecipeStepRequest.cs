using System.ComponentModel.DataAnnotations;

public record CreateRecipeStepRequest
{
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Описание должно содержать от 1 до 1000 символов.")]
    public string? Description { get; set; }
}
