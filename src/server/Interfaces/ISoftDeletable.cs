namespace server.Interfaces;

public interface ISoftDeletable
{
    Guid Id {get; set; }
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    DateTime LastModifiedAt { get; set; }
}