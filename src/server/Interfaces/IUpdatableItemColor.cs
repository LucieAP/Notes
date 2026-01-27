namespace server.Interfaces;

public interface IUpdatableItemColor
{
    Guid Id { get; }
    BackgroundColor BackgroundColor { get; set; }
    DateTime LastModifiedAt { get; set; }
}