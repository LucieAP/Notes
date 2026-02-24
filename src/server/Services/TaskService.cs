using Microsoft.EntityFrameworkCore;
using server.Interfaces;
using server.Services.Common;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GetTaskResponse>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.TaskGroup)
            .Select(t => new GetTaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Content,
                CheckBox = t.CheckBox,
                IsFavorite = t.IsFavorite,
                CreatedAt = t.CreatedAt,
                LastModifiedAt = t.LastModifiedAt,
                IsTrashed = t.IsTrashed,
                BackgroundColor = t.BackgroundColor,
                IsDeleted = t.IsDeleted,
                DeletedAt = t.DeletedAt,
                TaskGroup = t.TaskGroup != null ? new GroupResponse
                {
                    Id = t.TaskGroup.Id,
                    Title = t.TaskGroup.Title,
                    CreatedAt = t.TaskGroup.CreatedAt,
                    LastModifiedAt = t.TaskGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = t.User.Id,
                    Name = t.User.Name,
                    Email = t.User.Email,
                    Picture = t.User.Picture,
                    EmailVerified = t.User.EmailVerified,
                    LastLoginAt = t.User.LastLoginAt
                }
            })
            .ToListAsync(cancellationToken);

        return tasks;
    }

    public async Task<GetTaskResponse?> GetTaskByIdAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.TaskGroup)
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && !t.IsDeleted)
            .Select(t => new GetTaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Content,
                CheckBox = t.CheckBox,
                IsFavorite = t.IsFavorite,
                CreatedAt = t.CreatedAt,
                LastModifiedAt = t.LastModifiedAt,
                IsTrashed = t.IsTrashed,
                BackgroundColor = t.BackgroundColor,
                IsDeleted = t.IsDeleted,
                DeletedAt = t.DeletedAt,
                TaskGroup = t.TaskGroup != null ? new GroupResponse
                {
                    Id = t.TaskGroup.Id,
                    Title = t.TaskGroup.Title,
                    CreatedAt = t.TaskGroup.CreatedAt,
                    LastModifiedAt = t.TaskGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = t.User.Id,
                    Name = t.User.Name,
                    Email = t.User.Email,
                    Picture = t.User.Picture,
                    EmailVerified = t.User.EmailVerified,
                    LastLoginAt = t.User.LastLoginAt
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        _logger.LogInformation("Вывод всех найденных задач");

        return task;
    }

    public async Task<CreateTaskResponse> CreateTaskAsync(CreateTaskRequest createTaskRequest, Guid currentUserId, CancellationToken cancellationToken)
    {
        if (createTaskRequest.TaskGroupId != null)
        {
            var groupExists = await _context.TaskGroups
                .AnyAsync(
                    tg => tg.Id == createTaskRequest.TaskGroupId
                          && tg.CreatedBy == currentUserId
                          && !tg.IsDeleted,
                    cancellationToken);

            if (!groupExists)
            {
                throw new InvalidOperationException("Группа не найдена");
            }
        }

        var task = new Task
        {
            Id = Guid.NewGuid(),
            Title = createTaskRequest.Title,
            Content = createTaskRequest.Description,
            CheckBox = createTaskRequest.CheckBox,
            IsFavorite = createTaskRequest.IsFavorite,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsTrashed = false,
            BackgroundColor = createTaskRequest.BackgroundColor,
            IsDeleted = false,
            DeletedAt = null,
            CreatedBy = currentUserId,
            TaskGroupId = createTaskRequest.TaskGroupId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Создана новая задача c идентификатором {TaskId}", task.Id);

        return new CreateTaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Content,
            CheckBox = task.CheckBox,
            IsFavorite = task.IsFavorite,
            CreatedAt = task.CreatedAt,
            LastModifiedAt = task.LastModifiedAt,
            IsTrashed = task.IsTrashed,
            BackgroundColor = task.BackgroundColor,
            IsDeleted = task.IsDeleted,
            DeletedAt = task.DeletedAt,
            CreatedBy = task.CreatedBy,
            TaskGroupId = task.TaskGroupId
        };
    }

    public async Task<OperationResult<ToggleFavoriteResponse?>> ToggleFavoriteAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<ToggleFavoriteResponse?>.Failure("Задача не найдена", 404);
        }

        task.IsFavorite = !task.IsFavorite;
        task.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Кнопка избранное задачи {TaskId} переключена на {IsFavorite}", task.Id, task.IsFavorite);

        return OperationResult<ToggleFavoriteResponse?>.Success(new ToggleFavoriteResponse
        {
            Id = task.Id,
            IsFavorite = task.IsFavorite,
            LastModifiedAt = task.LastModifiedAt
        });
    }

    public async Task<OperationResult<ToggleCheckboxResponse?>> ToggleCheckboxAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<ToggleCheckboxResponse?>.Failure("Задача не найдена", 404);
        }

        task.CheckBox = !task.CheckBox;
        task.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Кнопка checkbox задачи {TaskId} переключена на {CheckBox}", task.Id, task.CheckBox);

        return OperationResult<ToggleCheckboxResponse?>.Success(new ToggleCheckboxResponse
        {
            Id = task.Id,
            CheckBox = task.CheckBox,
            LastModifiedAt = task.LastModifiedAt
        });
    }

    public async Task<OperationResult<UpdateItemResponse>> UpdateTaskAsync(Guid taskId, Guid currentUserId, UpdateItemRequest updateItemRequest, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<UpdateItemResponse>.Failure("Задача не найдена", 404);
        }

        var response = UpdateItemHelper.ApplyUpdate(task, "Task", currentUserId, updateItemRequest, _logger);
        
        if (response.IsSuccess && response.Value.WasUpdated)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        return response;
    }

    public async Task<OperationResult<UpdateColorResponse>> UpdateColorAsync(Guid taskId, Guid currentUserId, UpdateColorRequest updateColorRequest, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<UpdateColorResponse>.Failure("Задача не найдена", 404);
        }

        var response = UpdateItemHelper.ApplyColorUpdate(task, "Task", currentUserId, updateColorRequest, _logger);

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }

    public async Task<OperationResult> DeleteTaskByIdAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && t.IsDeleted != true)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult.Failure("Задача не найдена или уже удалена", 404);
        }

        var response = SoftDeleteHelper.SoftDelete(task, "Task", currentUserId, _logger);

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken); 
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult<TrashResponse>> TrashTaskAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
                .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && !t.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<TrashResponse>.Failure("Задача не найдена", 404);
        }

        task.IsTrashed = !task.IsTrashed;
        task.LastModifiedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Задача {TaskId} перемещена в корзину пользователем {UserId}", task.Id, currentUserId);

        return OperationResult<TrashResponse>.Success(new TrashResponse
        {
            Id = task.Id,
            IsTrashed = task.IsTrashed,
            LastModifiedAt = task.LastModifiedAt
        });
    }

    public async Task<OperationResult<CreateGroupResponse>> CreateTaskGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
    {
        var trimmedTitle = createGroupRequest.Title.Trim();

        if (string.IsNullOrEmpty(trimmedTitle))
        {
            return OperationResult<CreateGroupResponse>.Failure("Название группы задач не может быть пустым", 400);
        }

        var taskGroup = new TaskGroup
        {
            Id = Guid.NewGuid(),
            Title = trimmedTitle,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsDeleted = false,
            CreatedBy = currentUserId
        };

        _context.TaskGroups.Add(taskGroup);

        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult<CreateGroupResponse>.Success(new CreateGroupResponse
        {
            Id = taskGroup.Id,
            Title = taskGroup.Title,
            CreatedAt = taskGroup.CreatedAt,
            LastModifiedAt = taskGroup.LastModifiedAt
        });
    }

    public async Task<GroupResponse?> GetTaskGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var taskGroup = await _context.TaskGroups
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

        return taskGroup;
    }

    public async Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid taskId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var group = await _context.TaskGroups
            .Where(g => g.Id == groupId && g.CreatedBy == currentUserId && !g.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (group == null)
        {
            return OperationResult<AddToGroupResponse>.Failure("Группы задач с таким id не существует", 404);
        }

        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<AddToGroupResponse>.Failure("Задача не найдена", 404);
        }

        if (task.TaskGroupId == groupId)
        {
            return OperationResult<AddToGroupResponse>.Failure("Нельзя добавить в ту же группу задач", 400);
        }

        // Если задача была в другой группе, обновляем её timestamp
        TaskGroup? oldGroup = null;

        if (task.TaskGroupId.HasValue)
        {
            oldGroup = await _context.TaskGroups.FindAsync(task.TaskGroupId.Value, cancellationToken);
            if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
            {
                oldGroup.LastModifiedAt = DateTime.UtcNow;
            }
        }

        task.TaskGroupId = groupId;
        task.LastModifiedAt = DateTime.UtcNow;
        group.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Задача {taskId} добавлена в группу задач {groupId} (старая группа: {oldGroupId})",
                task.Id, groupId, oldGroup?.Id ?? Guid.Empty);

        return OperationResult<AddToGroupResponse>.Success(new AddToGroupResponse
        {
            ItemId = task.Id,
            ItemGroupId = task.TaskGroupId.Value,
            LastModifiedAt = task.LastModifiedAt,
        });
    }

    public async Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId && t.CreatedBy == currentUserId && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (task == null)
        {
            return OperationResult<RemoveFromGroupResponse>.Failure("Задача не найдена", 404);
        }

        if (!task.TaskGroupId.HasValue)
        {
            return OperationResult<RemoveFromGroupResponse>.Failure("Задача не находится в группе задач", 400);
        }

        // Обновляем timestamp старой группы
        var oldGroup = await _context.TaskGroups.FindAsync(task.TaskGroupId.Value);
        if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
        {
            oldGroup.LastModifiedAt = DateTime.UtcNow;
        }

        var oldGroupId = oldGroup?.Id ?? task.TaskGroupId.Value; // Сохраняем ID группы до обнуления

        task.TaskGroupId = null;
        task.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Задача {taskId} удалена из группы задач {oldGroupId}",
                task.Id, oldGroupId);

        return OperationResult<RemoveFromGroupResponse>.Success(new RemoveFromGroupResponse
        {
            ItemId = task.Id,
            ItemGroupId = oldGroupId,
            LastModifiedAt = task.LastModifiedAt,
        });
    }

    public async Task<OperationResult> DeleteTaskGroupAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var taskGroup = await _context.TaskGroups
            .Where(t => t.Id == groupId && t.CreatedBy == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (taskGroup == null)
        {
            return OperationResult.Failure("Группа задач не найдена", 404);
        }

        var response = SoftDeleteHelper.SoftDelete(taskGroup, "Task Group", currentUserId, _logger);

        // Сбрасываем TaskGroupId у всех задач в группе и обновляем их timestamps
        var tasksInGroup = await _context.Tasks
            .Where(t => t.TaskGroupId == groupId && t.CreatedBy == currentUserId && !t.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var task in tasksInGroup)
        {
            task.TaskGroupId = null;
            task.LastModifiedAt = DateTime.UtcNow;
        }

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Группа {groupId} удалена (soft-delete), задачи ({count}) очищены",
                groupId, tasksInGroup.Count);

        return OperationResult.Success();
    }
}