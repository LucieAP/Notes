public record TrashedTasksResponse
{
    public List<GetTaskResponse> Tasks { get; init; } = [];
}
