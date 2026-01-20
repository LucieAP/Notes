using System.ComponentModel.DataAnnotations;

public record UpdateItemRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public string? Title { get; set; }

    [StringLength(5000, ErrorMessage = "Описание не может превышать 5000 символов.")]
    public string? Description { get; set; }

    public BackgroundColor? BackgroundColor { get; set; }
}