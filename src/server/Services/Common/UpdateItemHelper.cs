using server.Interfaces;

namespace server.Services.Common;

internal class UpdateItemHelper
{
    public static OperationResult<UpdateNoteResponse> ApplyNoteUpdate(
        Note item,
        Guid currentUserId,
        UpdateNoteRequest updateNoteRequest,
        ILogger logger)
    {
        if (updateNoteRequest.Title == null &&
            updateNoteRequest.Content == null)
        {
            return OperationResult<UpdateNoteResponse>.Failure("Ни одного параметра не было передано", 400);
        }

        var wasUpdated = false;

        if (updateNoteRequest.Title != null)
        {
            var trimmedTitle = updateNoteRequest.Title.Trim();

            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                return OperationResult<UpdateNoteResponse>.Failure("Название не может быть пустым", 400);
            }

            if (trimmedTitle != item.Title)
            {
                item.Title = trimmedTitle;
                wasUpdated = true;
                logger.LogInformation(
                    "Название сущности {entityName} {itemId} обновлено пользователем {userId}",
                    "Note", item.Id, currentUserId
                );
            }
        }

        if (updateNoteRequest.Content != null)
        {
            var trimmedContent = string.IsNullOrWhiteSpace(updateNoteRequest.Content)
                ? null
                : updateNoteRequest.Content.Trim();

            if (trimmedContent != item.Content)
            {
                item.Content = trimmedContent;
                wasUpdated = true;
                logger.LogInformation(
                    "Описание сущности {entityName} {itemId} обновлено пользователем {userId}",
                    "Note", item.Id, currentUserId
                );
            }
        }

        if (wasUpdated)
        {
            item.LastModifiedAt = DateTime.UtcNow;
        }
        else
        {
            logger.LogInformation("Новые данные соответствуют старым, изменения не применены.");
        }

        return OperationResult<UpdateNoteResponse>.Success(new UpdateNoteResponse
        {
            Id = item.Id,
            Title = item.Title,
            Content = item.Content,
            LastModifiedAt = item.LastModifiedAt,
            WasUpdated = wasUpdated
        });
    }

    public static OperationResult<UpdateItemResponse> ApplyUpdate<T>(
        T item,
        string entityName,
        Guid currentUserId,
        UpdateItemRequest updateItemRequest,
        ILogger logger)
        where T : class, IUpdatableItem
    {
        if (updateItemRequest.Title == null && 
            updateItemRequest.Description == null)
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
            
            if (trimmedDescription != item.Content)
            {
                item.Content = trimmedDescription;
                wasUpdated = true;
                logger.LogInformation(
                    "Описание сущности {entityName} {itemId} обновлено пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if (wasUpdated)
        {
            item.LastModifiedAt = DateTime.UtcNow;
        }
        else
        {
            logger.LogInformation("Новые данные соответствуют старым, изменения не применены.");
        }

        return OperationResult<UpdateItemResponse>.Success(new UpdateItemResponse
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Content,
            LastModifiedAt = item.LastModifiedAt,
            WasUpdated = wasUpdated
        });
    }

    public static OperationResult<UpdateColorResponse> ApplyColorUpdate<T>(
        T item,
        string entityName,
        Guid currentUserId,
        UpdateColorRequest updateColorRequest,
        ILogger logger)
        where T : class, IUpdatableItemColor
    {
        if (updateColorRequest.BackgroundColor != item.BackgroundColor)
        {
            item.BackgroundColor = updateColorRequest.BackgroundColor;
            item.LastModifiedAt = DateTime.UtcNow;

            logger.LogInformation(
                "Цвет сущности {entityName} {itemId} изменен на {backgroundColor} пользователем {currentUserId}", 
                 entityName, item.Id, item.BackgroundColor, currentUserId);
        }

        return OperationResult<UpdateColorResponse>.Success(new UpdateColorResponse
        {
            BackgroundColor = item.BackgroundColor
        });
    }

    public static OperationResult<UpdateIngredientResponse> ApplyIngredientUpdate(
        Ingredient item,
        string entityName,
        Guid currentUserId,
        UpdateIngredientRequest updateIngredientRequest,
        ILogger logger)
    {
        if (updateIngredientRequest.Name == null && 
            updateIngredientRequest.Quantity == null &&
            updateIngredientRequest.Unit == null &&
            updateIngredientRequest.Note == null)
        {
            return OperationResult<UpdateIngredientResponse>.Failure("Ни одного параметра не было передано", 400);
        }

        var wasUpdated = false;

        if (updateIngredientRequest.Name != null)
        {
            var trimmedName = updateIngredientRequest.Name.Trim();  // Удаляем пробелы с начала и конца строки
    
            if (string.IsNullOrWhiteSpace(trimmedName))        // Проверяем пустоту строки
            {
                return OperationResult<UpdateIngredientResponse>.Failure("Название не может быть пустым", 400);
            }
            
            if (trimmedName != item.Name)
            {
                item.Name = trimmedName;
                wasUpdated = true;
                logger.LogInformation(
                    "Название сущности {entityName} {itemId} обновлено пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if  (updateIngredientRequest.Quantity != null)
        {
            if (updateIngredientRequest.Quantity.Value != item.Quantity)
            {
                item.Quantity = updateIngredientRequest.Quantity.Value;
                wasUpdated = true;
                logger.LogInformation(
                    "Количество сущности {entityName} {itemId} обновлено пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if  (updateIngredientRequest.Unit != null)
        {
            if (updateIngredientRequest.Unit.Value != item.Unit)
            {
                item.Unit = updateIngredientRequest.Unit.Value;
                wasUpdated = true;
                logger.LogInformation(
                    "Еденицы измерения сущности {entityName} {itemId} обновлены пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if (updateIngredientRequest.Note != null)
        {
            var trimmedNote = string.IsNullOrWhiteSpace(updateIngredientRequest.Note)  // Проверяем пустоту строки
                ? null 
                : updateIngredientRequest.Note.Trim();     // Удаляем пробелы с начала и конца строки
            
            if (trimmedNote != item.Note)
            {
                item.Note = trimmedNote;
                wasUpdated = true;
                logger.LogInformation(
                    "Описание сущности {entityName} {itemId} обновлено пользователем {userId}", 
                    entityName, item.Id, currentUserId
                );
            }
        }

        if (!wasUpdated)
        {
            logger.LogInformation("Новые данные соответствуют старым, изменения не применены.");
        }

        return OperationResult<UpdateIngredientResponse>.Success(new UpdateIngredientResponse
        {
            Id = item.Id,
            Name = item.Name,
            Quantity = item.Quantity,
            Unit = item.Unit,
            Note = item.Note,
            WasUpdated = wasUpdated
        });
    }
}