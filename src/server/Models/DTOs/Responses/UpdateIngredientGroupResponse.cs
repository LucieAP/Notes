public record UpdateIngredientGroupResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool WasUpdated { get; set; }
}