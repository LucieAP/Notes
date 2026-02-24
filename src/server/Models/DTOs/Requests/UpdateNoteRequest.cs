using System.ComponentModel.DataAnnotations;

public record UpdateNoteRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public string? Title { get; set; }

    [StringLength(65535, ErrorMessage = "Содержимое не может превышать 65535 символов.")]
    public string? Content { get; set; }
}

