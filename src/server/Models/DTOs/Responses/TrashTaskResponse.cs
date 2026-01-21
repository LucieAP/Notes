public record TrashTaskResponse
{
    public Guid Id { get; set; }
    public bool IsTrashed { get; set; }
    public DateTime LastModifiedAt { get; set; }
}