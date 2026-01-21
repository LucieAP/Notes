public record TogglePinResponse 
{
    public Guid Id { get; set; }
    public bool IsPinned { get; set; }
    public DateTime LastModifiedAt { get; set; }
}