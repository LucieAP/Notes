namespace server.Interfaces;

public interface IUpdatableItem
{
    Guid Id { get; }
    string Title { get; set; }
    string? Description { get; set; }
    DateTime LastModifiedAt { get; set; }
}