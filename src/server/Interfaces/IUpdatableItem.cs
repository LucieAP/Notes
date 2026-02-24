namespace server.Interfaces;

public interface IUpdatableItem
{
    Guid Id { get; }
    string Title { get; set; }
    string? Content { get; set; }
    DateTime LastModifiedAt { get; set; }
}