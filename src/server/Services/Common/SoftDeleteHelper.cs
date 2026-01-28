using server.Interfaces;

namespace server.Services.Common;

internal class SoftDeleteHelper
{
    public static OperationResult SoftDelete<T>(
        T entity,
        string entityName,
        Guid currentUserId,
        ILogger logger)
        where T: class, ISoftDeletable
    {
        if (entity.IsDeleted)
        {
            logger.LogInformation(
                "{EntityName} (id: {EntityId}) уже была помечена как удалённая",
                entityName, entity.Id);

            return OperationResult.Failure("Элемент уже удалён", 410);
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.LastModifiedAt = DateTime.UtcNow;

        logger.LogInformation(
            "{EntityName} (id: {EntityId}) был помечен как удалённый (soft-delete) пользователем {UserId}",
            entityName, entity.Id, currentUserId);

        return OperationResult.Success();
    }
}