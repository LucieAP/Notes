public record UpdateRecipeStepResponse
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public bool WasUpdated { get; set; }
}

