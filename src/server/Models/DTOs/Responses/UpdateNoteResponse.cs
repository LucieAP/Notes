using System.Text.Json;

public record UpdateNoteResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public JsonDocument? Content { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool WasUpdated { get; set; }
}
