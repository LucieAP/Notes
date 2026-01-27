public record RemoveFromGroupResponse
{
    public Guid ItemId { get; set; }
    public Guid ItemGroupId { get; set; }
    public DateTime LastModifiedAt { get; set; } 
}