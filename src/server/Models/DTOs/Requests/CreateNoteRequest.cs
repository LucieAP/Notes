using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public record CreateNoteRequest
{
    [Required(ErrorMessage = "Заголовок обязателен для заполнения.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Заголовок должен содержать от 1 до 100 символов.")]
    public required string Title { get; set; }

    public JsonDocument? Content { get; set; }
    public bool IsPinned { get; set; }
    public BackgroundColor BackgroundColor { get; set; } = BackgroundColor.Default;
    public Guid? NoteGroupId { get; set; }
}
