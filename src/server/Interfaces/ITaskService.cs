namespace server.Interfaces;

public interface ITaskService
{
    public Task<IEnumerable<GetTaskResponse>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    public Task<GetTaskResponse?> GetTaskByIdAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<CreateTaskResponse> CreateTaskAsync(CreateTaskRequest createTaskRequest, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<ToggleFavoriteResponse?>> ToggleFavoriteAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<ToggleCheckboxResponse?>> ToggleCheckboxAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateItemResponse>> UpdateTaskAsync(Guid taskId, Guid currentUserId, UpdateItemRequest updateItemRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateColorResponse>> UpdateColorAsync(Guid noteId, Guid currentUserId, UpdateColorRequest updateColorRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteTaskByIdAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<TrashResponse>> TrashTaskAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<CreateGroupResponse>> CreateTaskGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default);
    public Task<GroupResponse?> GetTaskGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid taskId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteTaskGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
}
