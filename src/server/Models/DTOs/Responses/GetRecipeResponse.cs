public record GetRecipeResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public bool IsTrashed { get; set; }
    public bool IsDeleted { get; set; } 
    public DateTime? DeletedAt { get; set; }
    public GroupResponse? RecipeGroup { get; set; }
    public GetUserResponse CreatedBy { get; set; }
    public List<GetIngredientResponse> Ingredients {get; set; } = new();
}