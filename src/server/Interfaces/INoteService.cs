namespace server.Interfaces
{
    public interface INoteService
    {
        public Task<IEnumerable<GetNoteResponse>> GetAllNotesAsync(CancellationToken cancellationToken = default);
        public Task<GetNoteResponse?> GetNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequest createNoteRequest, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult<TogglePinResponse?>> TogglePinAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult<UpdateItemResponse>> UpdateNoteAsync(Guid noteId, Guid currentUserId, UpdateItemRequest updateItemRequest, CancellationToken cancellationToken = default);
        public Task<OperationResult> DeleteNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult<TrashNoteResponse>> TrashNoteAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult<CreateGroupResponse>> CreateNoteGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default);
        public Task<GroupResponse?> GetNoteGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid noteId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default);
        public Task<OperationResult> DeleteNoteGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default);
    }
}