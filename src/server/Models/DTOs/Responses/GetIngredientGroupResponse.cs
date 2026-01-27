public record GetIngredientGroupResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RecipeId { get; set; }
}
