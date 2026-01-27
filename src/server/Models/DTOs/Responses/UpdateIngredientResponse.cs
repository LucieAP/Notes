public record UpdateIngredientResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public float? Quantity { get; set; } 
    public Unit? Unit { get; set; }
    public string? Note { get; set; } 
    public bool WasUpdated { get; set; }
}