public record GetNoteResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public BackgroundColor BackgroundColor { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime? DeletedAt { get; set; }
    public GroupResponse? NoteGroup { get; set; }
    public GetUserResponse CreatedBy { get; set; }
}