public record CreateIngredientRequest
{
    public string? Name { get; set; } 
    public float? Quantity { get; set; } 
    public Unit? Unit { get; set; }
    public string? Note { get; set; } 
    public Guid? IngredientGroupId { get; set; } 
}