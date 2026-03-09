public record TrashedRecipesResponse
{
    public List<GetRecipeResponse> Recipes { get; init; } = [];
}
