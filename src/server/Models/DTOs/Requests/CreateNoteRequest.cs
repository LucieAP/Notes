public record CreateNoteRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsPinned { get; set; }

    public BackgroundColor BackgroundColor { get; set; } = BackgroundColor.Default;

    public Guid? NoteGroupId  {get; set;}
}