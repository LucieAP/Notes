public record PinnedNotesResponse
{
    public List<GetNoteResponse> Notes { get; init; } = [];
}
