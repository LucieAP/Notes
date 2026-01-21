public record ToggleFavoriteResponse
{
    public Guid Id { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime LastModifiedAt { get; set; }
}