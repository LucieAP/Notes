public record AddToGroupResponse
{
    public Guid NoteId { get; set; }
    public Guid NoteGroupId { get; set; }
    public DateTime LastModifiedAt { get; set; } 
}