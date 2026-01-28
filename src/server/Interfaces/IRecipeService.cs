namespace server.Interfaces;

public interface IRecipeService
{
    public Task<IEnumerable<GetRecipeResponse>> GetAllRecipesAsync(CancellationToken cancellationToken = default);
    public Task<GetRecipeResponse?> GetRecipeByIdAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<CreateRecipeResponse> CreateRecipeAsync(CreateRecipeRequest createRecipeRequest, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<ToggleFavoriteResponse?>> ToggleFavoriteAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateItemResponse>> UpdateRecipeAsync(Guid recipeId, Guid currentUserId, UpdateItemRequest updateItemRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteRecipeByIdAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<TrashResponse>> TrashRecipeByIdAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<CreateGroupResponse>> CreateRecipeGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default);
    public Task<GroupResponse?> GetRecipeGroupByIdAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid recipeId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteRecipeGroupByIdAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<CreateIngredientResponse>> CreateIngredientAsync(Guid recipeId, CreateIngredientRequest createIngredientRequest, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<GetIngredientResponse?> GetIngredientByIdAsync(Guid ingredientId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateIngredientResponse>> UpdateIngredientAsync(Guid ingredientId, Guid currentUserId, UpdateIngredientRequest updateIngredientRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteIngredientByIdAsync(Guid ingredientId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<CreateIngredientGroupResponse>> CreateIngredientGroupAsync(Guid recipeId, CreateIngredientGroupRequest createIngredientGroupRequest, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<GetIngredientGroupResponse?> GetIngredientGroupByIdAsync(Guid ingredientGroupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateIngredientGroupResponse>> UpdateIngredientGroupAsync(Guid ingredientGroupId, Guid currentUserId, UpdateIngredientGroupRequest updateIngredientGroupRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteIngredientGroupByIdAsync(Guid ingredientGroupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<GetRecipeStepResponse?> GetRecipeStepAsync(Guid recipeStepId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<CreateRecipeStepResponse> CreateRecipeStepAsync(Guid recipeId, CreateRecipeStepRequest createRecipeStepRequest, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateRecipeStepResponse>> UpdateRecipeStepAsync(Guid recipeStepId, Guid currentUserId, UpdateRecipeStepRequest updateRecipeStepRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteRecipeStepByIdAsync(Guid recipeStepId, Guid currentUserId, CancellationToken cancellationToken = default);
}