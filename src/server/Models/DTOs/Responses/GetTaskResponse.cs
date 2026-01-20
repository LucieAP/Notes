public record GetTaskResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool CheckBox {get; set; }
    public bool IsFavorite{ get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public BackgroundColor BackgroundColor { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime? DeletedAt { get; set; }
    public GroupResponse? TaskGroup { get; set; }
    public GetUserResponse CreatedBy { get; set; }
}