using Microsoft.EntityFrameworkCore;
using server.Interfaces;

public class NoteService : INoteService
{
    private readonly AppDbContext _context;
    private readonly ILogger<NoteService> _logger;

    public NoteService(AppDbContext context, ILogger<NoteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GetNoteResponse>> GetAllNotesAsync(CancellationToken cancellationToken = default)
    {
        var notes = await _context.Notes
            .AsNoTracking()
            .Include(n => n.User)
            .Include(n => n.NoteGroup)
            .Select(n => new GetNoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                IsPinned = n.IsPinned,
                CreatedAt = n.CreatedAt,
                LastModifiedAt = n.LastModifiedAt,
                IsTrashed = n.IsTrashed,
                BackgroundColor = n.BackgroundColor,
                IsDeleted = n.IsDeleted,
                DeletedAt = n.DeletedAt,
                NoteGroup = n.NoteGroup != null ? new GroupResponse
                {
                    Id = n.NoteGroup.Id,
                    Title = n.NoteGroup.Title,
                    CreatedAt = n.NoteGroup.CreatedAt,
                    LastModifiedAt = n.NoteGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = n.User.Id,
                    Name = n.User.Name,
                    Email = n.User.Email,
                    Picture = n.User.Picture,
                    EmailVerified = n.User.EmailVerified,
                    LastLoginAt = n.User.LastLoginAt
                }
            })
            .ToListAsync(cancellationToken);
        
        return notes;
    }

    public async Task<GetNoteResponse?> GetNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .AsNoTracking()
            .Include(n => n.User)
            .Include(n => n.NoteGroup)
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId)
            .Select(n => new GetNoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                IsPinned = n.IsPinned,
                CreatedAt = n.CreatedAt,
                LastModifiedAt = n.LastModifiedAt,
                IsTrashed = n.IsTrashed,
                BackgroundColor = n.BackgroundColor,
                IsDeleted = n.IsDeleted,
                DeletedAt = n.DeletedAt,
                NoteGroup = n.NoteGroup != null ? new GroupResponse
                {
                    Id = n.NoteGroup.Id,
                    Title = n.NoteGroup.Title,
                    CreatedAt = n.NoteGroup.CreatedAt,
                    LastModifiedAt = n.NoteGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = n.User.Id,
                    Name = n.User.Name,
                    Email = n.User.Email,
                    Picture = n.User.Picture,
                    EmailVerified = n.User.EmailVerified,
                    LastLoginAt = n.User.LastLoginAt
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        _logger.LogInformation("Вывод всех найденных заметок");

        return note;
    }

    public async Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequest createNoteRequest, Guid currentUserId, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Title = createNoteRequest.Title,
            Description = createNoteRequest.Description,
            IsPinned = createNoteRequest.IsPinned,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsTrashed = false,
            BackgroundColor = createNoteRequest.BackgroundColor,
            IsDeleted = false,
            DeletedAt = null,
            CreatedBy = currentUserId,
            NoteGroupId = createNoteRequest.NoteGroupId
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Создана новая заметка c идентификатором {NoteId}", note.Id);

        return new CreateNoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Description = note.Description,
            IsPinned = note.IsPinned,
            CreatedAt = note.CreatedAt,
            LastModifiedAt = note.LastModifiedAt,
            IsTrashed = note.IsTrashed,
            BackgroundColor = note.BackgroundColor,
            IsDeleted = note.IsDeleted,
            DeletedAt = note.DeletedAt,
            CreatedBy = note.CreatedBy,
            NoteGroupId = note.NoteGroupId
        };
    } 

    public async Task<OperationResult<TogglePinResponse?>> TogglePinAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult<TogglePinResponse?>.Failure("Заметка не найдена", 404);
        }

        note.IsPinned = !note.IsPinned;
        note.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Кнопка закрепления заметки {NoteId} переключена на {IsPinned}", note.Id, note.IsPinned);

        return OperationResult<TogglePinResponse?>.Success(new TogglePinResponse
        {
            Id = note.Id,
            IsPinned = note.IsPinned,
            LastModifiedAt = note.LastModifiedAt
        });
    }

    public async Task<OperationResult<UpdateItemResponse>> UpdateNoteAsync(Guid noteId, Guid currentUserId, UpdateItemRequest updateItemRequest, CancellationToken cancellationToken = default)
    {
        if (updateItemRequest.Title == null && 
            updateItemRequest.Description == null && 
            updateItemRequest.BackgroundColor == null)
        {
            return OperationResult<UpdateItemResponse>.Failure("Ни одного параметра не было передано", 400);
        }

        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult<UpdateItemResponse>.Failure("Заметка не найдена", 404);
        }

        var wasUpdated = false;

        if (updateItemRequest.Title != null)
        {
            var trimmedTitle = updateItemRequest.Title.Trim();  // Удаляем пробелы с начала и конца строки
    
            if (string.IsNullOrWhiteSpace(trimmedTitle))        // Проверяем пустоту строки
            {
                return OperationResult<UpdateItemResponse>.Failure("Название не может быть пустым", 400);
            }
            
            if (trimmedTitle != note.Title)
            {
                note.Title = trimmedTitle;
                wasUpdated = true;
                _logger.LogInformation(
                    "Название заметки {NoteId} обновлено пользователем {UserId}", 
                    note.Id, currentUserId
                );
            }
        }
        if (updateItemRequest.Description != null)
        {
            var trimmedDescription = string.IsNullOrWhiteSpace(updateItemRequest.Description)  // Проверяем пустоту строки
                ? null 
                : updateItemRequest.Description.Trim();     // Удаляем пробелы с начала и конца строки
            
            if (trimmedDescription != note.Description)
            {
                note.Description = trimmedDescription;
                wasUpdated = true;
                _logger.LogInformation(
                    "Описание заметки {NoteId} обновлено пользователем {UserId}", 
                    note.Id, currentUserId
                );
            }
        }
        if (updateItemRequest.BackgroundColor.HasValue && 
            updateItemRequest.BackgroundColor.Value != note.BackgroundColor)
        {
            note.BackgroundColor = updateItemRequest.BackgroundColor.Value;
            wasUpdated = true;
            _logger.LogInformation("Цвет заметки {NoteId} изменен на {BackgroundColor}", note.Id, note.BackgroundColor);
        }

        if (wasUpdated)
        {
            note.LastModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken); 
        }
        else
        {
            _logger.LogInformation("Новые данные соответсвуют старым, изменения не применены.");
        }

        return OperationResult<UpdateItemResponse>.Success(new UpdateItemResponse
        {
            Id = note.Id,
            Title = note.Title,
            Description = note.Description,
            BackgroundColor = note.BackgroundColor,
            LastModifiedAt = note.LastModifiedAt,
            WasUpdated = wasUpdated
        });
    } 

    public async Task<OperationResult> DeleteNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && n.IsDeleted != true)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            // return NotFound( new {message = "Заметка не найдена"});
            return OperationResult.Failure("Заметка не найдена", 404);
        }

        note.LastModifiedAt = DateTime.UtcNow;

        // Мягкое удаление
        note.IsDeleted = true;
        note.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Note {NotesId} была удалена пользователем {Userid}", noteId, currentUserId);

        return OperationResult.Success();
    }

    public async Task<OperationResult<TrashNoteResponse>> TrashNoteAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
                .Where(n => n.Id == noteId && n.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
                
        if (note == null)
        {
            return OperationResult<TrashNoteResponse>.Failure("Заметка не найдена", 404);
        }

        note.IsTrashed = !note.IsTrashed;
        note.LastModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Заметка {NoteId} перемещена в корзину пользователем {UserId}", note.Id, currentUserId);
    
        return OperationResult<TrashNoteResponse>.Success( new TrashNoteResponse
        {
            Id = note.Id,
            IsTrashed = note.IsTrashed,
            LastModifiedAt = note.LastModifiedAt
        });
    }

    public async Task<OperationResult<CreateGroupResponse>> CreateNoteGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
    {
        var trimmedTitle = createGroupRequest.Title.Trim();

        if (string.IsNullOrEmpty(trimmedTitle))
        {
            return OperationResult<CreateGroupResponse>.Failure("Название группы не может быть пустым", 400);
        }
        
        var noteGroup = new NoteGroup
        {
            Id = Guid.NewGuid(),
            Title = trimmedTitle,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsDeleted = false,
            CreatedBy = currentUserId
        };

        _context.NoteGroups.Add(noteGroup);
        
        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult<CreateGroupResponse>.Success(new CreateGroupResponse
        {
            Id = noteGroup.Id,
            Title = noteGroup.Title,
            CreatedAt = noteGroup.CreatedAt,
            LastModifiedAt = noteGroup.LastModifiedAt
        });
    }

    public async Task<GroupResponse?> GetNoteGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var noteGroup = await _context.NoteGroups
            .AsNoTracking()
            .Where(g => g.Id == groupId && g.CreatedBy == currentUserId && !g.IsDeleted)
            .Select(g => new GroupResponse
            {
                Id = g.Id,
                Title = g.Title,
                CreatedAt = g.CreatedAt,
                LastModifiedAt = g.LastModifiedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return noteGroup;
    }

    public async Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid noteId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var group = await _context.NoteGroups
            .Where(g => g.Id == groupId && g.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (group == null)
        {
            return OperationResult<AddToGroupResponse>.Failure("Группы с таким id не существует", 404);
        }

        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult<AddToGroupResponse>.Failure("Записка не найдена", 404);
        }

        if (note.NoteGroupId == groupId)
        {
            return OperationResult<AddToGroupResponse>.Failure("Нельзя добавить в ту же группу", 400);
        }

        // Если заметка была в другой группе, обновляем её timestamp
        NoteGroup? oldGroup = null;

        if (note.NoteGroupId.HasValue)
        {
            oldGroup = await _context.NoteGroups.FindAsync(note.NoteGroupId.Value, cancellationToken);
            if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
            {
                oldGroup.LastModifiedAt = DateTime.UtcNow;
            }
        }

        note.NoteGroupId = groupId;
        note.LastModifiedAt = DateTime.UtcNow;
        group.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Записка {noteId} добавлена в группу {groupId} (старая группа: {oldGroupId})", 
                note.Id, groupId, oldGroup?.Id ?? Guid.Empty);

        return OperationResult<AddToGroupResponse>.Success( new AddToGroupResponse
        {
            NoteId = note.Id,
            NoteGroupId = note.NoteGroupId.Value,
            LastModifiedAt = note.LastModifiedAt,
        });
    }

    public async Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult<RemoveFromGroupResponse>.Failure("Записка не найдена", 404);
        }

        if (!note.NoteGroupId.HasValue)
        {
            return OperationResult<RemoveFromGroupResponse>.Failure("Заметка не находится в группе", 400);
        }

        // Обновляем timestamp старой группы
        var oldGroup = await _context.NoteGroups.FindAsync(note.NoteGroupId.Value);
        if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
        {
            oldGroup.LastModifiedAt = DateTime.UtcNow;
        }

        var oldGroupId = oldGroup?.Id ?? note.NoteGroupId.Value; // Сохраняем ID группы до обнуления

        note.NoteGroupId = null;
        note.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Заметка {noteId} удалена из группы {oldGroupId}",
                note.Id, oldGroupId);

        return OperationResult<RemoveFromGroupResponse>.Success( new RemoveFromGroupResponse
        {
            NoteId = note.Id,
            NoteGroupId = oldGroupId,
            LastModifiedAt = note.LastModifiedAt,
        });
    }

    public async Task<OperationResult> DeleteNoteGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var noteGroup = await _context.NoteGroups
            .Where(n => n.Id == groupId && n.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (noteGroup == null)
        {
            return OperationResult.Failure("Группа не найдена", 404);
        }

        noteGroup.IsDeleted = true;
        noteGroup.DeletedAt = DateTime.UtcNow;
        noteGroup.LastModifiedAt = DateTime.UtcNow;

        // Сбрасываем NoteGroupId у всех заметок в группе и обновляем их timestamps
        var notesInGroup = await _context.Notes
            .Where(n => n.NoteGroupId == groupId && n.CreatedBy == currentUserId)
            .ToListAsync(cancellationToken);

        foreach (var note in notesInGroup)
        {
            note.NoteGroupId = null;
            note.LastModifiedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Группа {groupId} удалена (soft-delete), заметки ({count}) очищены", 
                groupId, notesInGroup.Count);

        return OperationResult.Success();
    }  
}