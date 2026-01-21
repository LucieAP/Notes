public record UpdateItemResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public BackgroundColor? BackgroundColor { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool WasUpdated { get; set; }
}