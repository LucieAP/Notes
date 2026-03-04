using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public record UpdateNoteRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public string? Title { get; set; }

    public JsonDocument? Content { get; set; }
}

