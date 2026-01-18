public record UpdateNoteRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public BackgroundColor? BackgroundColor { get; set; }
}