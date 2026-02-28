public record TrashedNotesResponse
{
    public List<GetNoteResponse> Notes { get; init; } = [];
}
