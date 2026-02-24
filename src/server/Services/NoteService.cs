using Microsoft.EntityFrameworkCore;
using server.Interfaces;
using server.Services.Common;

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
                Content = n.Content,
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
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && !n.IsDeleted)
            .Select(n => new GetNoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
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
        if (createNoteRequest.NoteGroupId != null)
        {
            var groupExists = await _context.NoteGroups
                .AnyAsync(
                    ng => ng.Id == createNoteRequest.NoteGroupId
                          && ng.CreatedBy == currentUserId
                          && !ng.IsDeleted,
                    cancellationToken);

            if (!groupExists)
            {
                throw new InvalidOperationException("Группа не найдена");
            }
        }

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Title = createNoteRequest.Title,
            Content = createNoteRequest.Content,
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
            Content = note.Content,
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

    public async Task<OperationResult<UpdateNoteResponse>> UpdateNoteAsync(Guid noteId, Guid currentUserId, UpdateNoteRequest updateNoteRequest, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && !n.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult<UpdateNoteResponse>.Failure("Заметка не найдена", 404);
        }

        var response = UpdateItemHelper.ApplyNoteUpdate(note, currentUserId, updateNoteRequest, _logger);

        if (response.IsSuccess && response.Value.WasUpdated)
        {
            await _context.SaveChangesAsync(cancellationToken); 
        }

        return response;
    }

    public async Task<OperationResult<UpdateColorResponse>> UpdateColorAsync(Guid noteId, Guid currentUserId, UpdateColorRequest updateColorRequest, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && !n.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult<UpdateColorResponse>.Failure("Заметка не найдена", 404);
        }

        var response = UpdateItemHelper.ApplyColorUpdate(note, "Note", currentUserId, updateColorRequest, _logger);

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }

    public async Task<OperationResult> DeleteNoteByIdAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && n.IsDeleted != true)
            .FirstOrDefaultAsync(cancellationToken);

        if (note == null)
        {
            return OperationResult.Failure("Заметка не найдена или уже удалена", 404);
        }

        var response = SoftDeleteHelper.SoftDelete(note, "Note", currentUserId, _logger);

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken); 
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult<TrashResponse>> TrashNoteAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
                .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && !n.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
                
        if (note == null)
        {
            return OperationResult<TrashResponse>.Failure("Заметка не найдена", 404);
        }

        note.IsTrashed = !note.IsTrashed;
        note.LastModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Заметка {NoteId} перемещена в корзину пользователем {UserId}", note.Id, currentUserId);
    
        return OperationResult<TrashResponse>.Success( new TrashResponse
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
            .Where(g => g.Id == groupId && g.CreatedBy == currentUserId && !g.IsDeleted)
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
            ItemId = note.Id,
            ItemGroupId = note.NoteGroupId.Value,
            LastModifiedAt = note.LastModifiedAt,
        });
    }

    public async Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid noteId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes
            .Where(n => n.Id == noteId && n.CreatedBy == currentUserId && !n.IsDeleted)
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
            ItemId = note.Id,
            ItemGroupId = oldGroupId,
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

        var response = SoftDeleteHelper.SoftDelete(noteGroup, "Note Group", currentUserId, _logger);

        // Сбрасываем NoteGroupId у всех заметок в группе и обновляем их timestamps
        var notesInGroup = await _context.Notes
            .Where(n => n.NoteGroupId == groupId && n.CreatedBy == currentUserId && !n.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var note in notesInGroup)
        {
            note.NoteGroupId = null;
            note.LastModifiedAt = DateTime.UtcNow;
        }

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Группа {groupId} удалена (soft-delete), заметки ({count}) очищены", 
                groupId, notesInGroup.Count);

        return OperationResult.Success();
    }  
}