using System.ComponentModel.DataAnnotations;

public record UpdateIngredientGroupRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public string? Title { get; set; }
}
