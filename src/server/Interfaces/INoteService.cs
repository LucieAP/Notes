namespace server.Interfaces;

public interface INoteService
{
    public Task<IEnumerable<GetNoteResponse>> GetAllNotesAsync(Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<GetNoteResponse?> GetNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequest createNoteRequest, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<TogglePinResponse?>> TogglePinAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateNoteResponse>> UpdateNoteAsync(Guid noteId, Guid currentUserId, UpdateNoteRequest updateNoteRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult<UpdateColorResponse>> UpdateColorAsync(Guid noteId, Guid currentUserId, UpdateColorRequest updateColorRequest, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<TrashResponse>> TrashNoteAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<TrashedNotesResponse>> GetTrashedNotesAsync(Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<CreateGroupResponse>> CreateNoteGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default);
    public Task<GroupResponse?> GetNoteGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid noteId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
    public Task<OperationResult> DeleteNoteGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
}
