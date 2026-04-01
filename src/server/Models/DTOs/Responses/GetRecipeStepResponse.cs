public record GetRecipeStepResponse
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RecipeId { get; set; }
}
