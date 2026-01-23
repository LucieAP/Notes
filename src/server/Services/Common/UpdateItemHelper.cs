using server.Interfaces;

namespace server.Services.Common;

internal class UpdateItemHelper
{
    public static OperationResult<UpdateItemResponse> ApplyUpdate<T>(
        T item,
        string entityName,
        Guid currentUserId,
        UpdateItemRequest updateItemRequest,
        ILogger logger)
        where T : class, IUpdatableItem
    {
        if (updateItemRequest.Title == null && 
            updateItemRequest.Description == null && 
            updateItemRequest.BackgroundColor == null)
        {
            return OperationResult<UpdateItemResponse>.Failure("Ни одного параметра не было передано", 400);
        }

        var wasUpdated = false;

        if (updateItemRequest.Title != null)
        {
            var trimmedTitle = updateItemRequest.Title.Trim();  // Удаляем пробелы с начала и конца строки
    
            if (string.IsNullOrWhiteSpace(trimmedTitle))        // Проверяем пустоту строки
            {
                return OperationResult<UpdateItemResponse>.Failure("Название не может быть пустым", 400);
            }
            
            if (trimmedTitle != item.Title)
            {
                item.Title = trimmedTitle;
                wasUpdated = true;
                logger.LogInformation(
                    "Название сущности {entityName} {itemId} обновлено пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if (updateItemRequest.Description != null)
        {
            var trimmedDescription = string.IsNullOrWhiteSpace(updateItemRequest.Description)  // Проверяем пустоту строки
                ? null 
                : updateItemRequest.Description.Trim();     // Удаляем пробелы с начала и конца строки
            
            if (trimmedDescription != item.Description)
            {
                item.Description = trimmedDescription;
                wasUpdated = true;
                logger.LogInformation(
                    "Описание сущности {entityName} {itemId} обновлено пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if (updateItemRequest.BackgroundColor.HasValue && 
            updateItemRequest.BackgroundColor.Value != item.BackgroundColor)
        {
            item.BackgroundColor = updateItemRequest.BackgroundColor.Value;
            wasUpdated = true;
            logger.LogInformation(
                "Цвет сущности {entityName} {itemId} изменен на {backgroundColor}", 
                entityName, item.Id, item.BackgroundColor);
        }

        if (wasUpdated)
        {
            item.LastModifiedAt = DateTime.UtcNow;
        }
        else
        {
            logger.LogInformation("Новые данные соответсвуют старым, изменения не применены.");
        }

        return OperationResult<UpdateItemResponse>.Success(new UpdateItemResponse
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            BackgroundColor = item.BackgroundColor,
            LastModifiedAt = item.LastModifiedAt,
            WasUpdated = wasUpdated
        });
    }
}