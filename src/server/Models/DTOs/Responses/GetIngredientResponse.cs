public record GetIngredientResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public float Quantity { get; set; } 
    public Unit Unit { get; set; }
    public string? Note { get; set; } 
    public DateTime CreatedAt { get; set; }
    public Guid? IngredientGroupId { get; set; } 
    public Guid RecipeId { get; set; }
}
