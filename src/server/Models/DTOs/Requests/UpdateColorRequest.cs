using System.ComponentModel.DataAnnotations;

public record UpdateColorRequest
{
    [Required(ErrorMessage = "Цвет фона обязателен для заполнения.")]
    public BackgroundColor BackgroundColor { get; set; }
}