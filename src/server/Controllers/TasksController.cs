using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace server.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TasksController> _logger;
        private readonly UserService _userService;

        public TasksController(AppDbContext context, ILogger<TasksController> logger, UserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetTaskResponse>>> GetTasks(CancellationToken cancellationToken = default)
        {
            var tasks = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.TaskGroup)
                .Select(t => new GetTaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
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

            return Ok(tasks);
        } 

        // GET: api/tasks/id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetTaskResponse>> GetTask([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.TaskGroup)
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .Select(t => new GetTaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
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
            
            if (task == null)
            {
                return NotFound( new {message = "Задача не найдена"});
            }

            _logger.LogInformation("Вывод всех найденных задач");

            return Ok(task);
        }

        // POST: api/tasks/create
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest, CancellationToken cancellationToken = default)
        {

            var currentUserId = _userService.GetUserId(User);

            var task = new Task
            {
                Id = Guid.NewGuid(),
                Title = createTaskRequest.Title,
                Description = createTaskRequest.Description,
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

            var response = new CreateTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
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

            _logger.LogInformation("Создана новая задача {TaskId}", task.Id);

            return CreatedAtAction(
                nameof(GetTask),
                new {id = task.Id},
                response);
        }

        // PATCH: api/task/{id}/favorite
        [HttpPatch("{id}/favorite")]
        [Authorize]
        public async Task<IActionResult> ToggleTaskFavoriteStatus([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (task == null)
            {
                return NotFound( new {message = "Задача не найдена"});
            }

            task.IsFavorite = !task.IsFavorite;
            task.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Кнопка избранное задачи {TaskId} переключена на {IsFavorite}", task.Id, task.IsFavorite);

            return Ok(new{
                id = task.Id,
                isFavorite = task.IsFavorite,
                lastModifiedAt = task.LastModifiedAt
            });
        }

        // PATCH: api/task/{id}/checkbox
        [HttpPatch("{id}/checkbox")]
        [Authorize]
        public async Task<IActionResult> ToggleTaskCheckboxStatus([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (task == null)
            {
                return NotFound( new {message = "Задача не найдена"});
            }

            task.CheckBox = !task.CheckBox;
            task.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Кнопка checkbox задачи {TaskId} переключена на {CheckBox}", task.Id, task.CheckBox);

            return Ok(new{
                id = task.Id,
                checkBox = task.CheckBox,
                lastModifiedAt = task.LastModifiedAt
            });
        }

        // PATCH: api/task/{id}/
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateItemRequest updateItemRequest , CancellationToken cancellationToken = default)
        {
            if (updateItemRequest.Title == null && 
                updateItemRequest.Description == null && 
                updateItemRequest.BackgroundColor == null)
            {
                return BadRequest(new { message = "Ни одного параметра не было передано" });
            }

            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (task == null)
            {
                return NotFound( new {message = "Задача не найдена"});
            }

            var wasUpdated = false;

            if (updateItemRequest.Title != null)
            {
                var trimmedTitle = updateItemRequest.Title.Trim();  // Удаляем пробелы с начала и конца строки
        
                if (string.IsNullOrWhiteSpace(trimmedTitle))        // Проверяем пустоту строки
                {
                    return BadRequest(new { message = "Заголовок не может быть пустым" });
                }
                
                if (trimmedTitle != task.Title)
                {
                        task.Title = trimmedTitle;
                    wasUpdated = true;
                    _logger.LogInformation(
                        "Заголовок задачи {TaskId} обновлено пользователем {UserId}", 
                        task.Id, currentUserId
                    );
                }
            }
            if (updateItemRequest.Description != null)
            {
                var trimmedDescription = string.IsNullOrWhiteSpace(updateItemRequest.Description)  // Проверяем пустоту строки
                    ? null 
                    : updateItemRequest.Description.Trim();     // Удаляем пробелы с начала и конца строки
                
                if (trimmedDescription != task.Description)
                {
                    task.Description = trimmedDescription;
                    wasUpdated = true;
                    _logger.LogInformation(
                        "Описание задачи {TaskId} обновлено пользователем {UserId}", 
                        task.Id, currentUserId
                    );
                }
            }
            if (updateItemRequest.BackgroundColor.HasValue && 
                updateItemRequest.BackgroundColor.Value != task.BackgroundColor)
            {
                task.BackgroundColor = updateItemRequest.BackgroundColor.Value;
                wasUpdated = true;
                _logger.LogInformation("Цвет задачи {TaskId} изменен на {BackgroundColor}", task.Id, task.BackgroundColor);
            }

            if (wasUpdated)
            {
                task.LastModifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken); 
            }
            else
            {
                _logger.LogInformation("Новые данные соответсвуют старым, изменения не применены.");
            }

            return Ok(new{
                id = task.Id,
                title = task.Title,
                description = task.Description,
                backgroundColor = task.BackgroundColor,
                lastModifiedAt = task.LastModifiedAt,
                wasUpdated = wasUpdated
            });
        }

         // DELETE: api/task/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTaskById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId && t.IsDeleted != true)
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
            {
                return NotFound( new {message = "Задача не найдена"});
            }

            task.LastModifiedAt = DateTime.UtcNow;

            // Мягкое удаление
            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Task {TaskId} была удалена пользователем {UserId}", id, currentUserId);

            return NoContent();       
        }

        // PATCH: api/task/{id}/trash
        [HttpPatch("{id}/trash")]
        [Authorize]
        public async Task<IActionResult> TrashTask([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
                
            if (task == null)
            {
                return NotFound( new {message = "Задача не найдена"});
            }

            task.IsTrashed = !task.IsTrashed;
            task.LastModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Задача {TaskId} перемещена в корзину пользователем {UserId}", task.Id, currentUserId);

            return Ok( new {
                id = task.Id,
                isTrashed = task.IsTrashed,
                lastModifiedAt = task.LastModifiedAt
            });
        }

        // POST: api/task/group/create
        [HttpPost("group/create")]
        [Authorize]
        public async Task<IActionResult> CreateTaskGroup([FromBody]CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
        {

            var currentUserId = _userService.GetUserId(User);
            var trimmedTitle = createGroupRequest.Title.Trim();

            if (string.IsNullOrEmpty(trimmedTitle))
            {
                return BadRequest(new { message = "Название группы задач не может быть пустым" });
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

            var response = new CreateGroupResponse
            {
                Id = taskGroup.Id,
                Title = taskGroup.Title,
                CreatedAt = taskGroup.CreatedAt,
                LastModifiedAt = taskGroup.LastModifiedAt
            };

            return CreatedAtAction(
                nameof(GetTaskGroup),
                new { id = taskGroup.Id },
                response
            );
        }

        // GET: api/task/group/{id}
        [HttpGet("group/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var taskGroup = await _context.TaskGroups
                .AsNoTracking()
                .Where(g => g.Id == id && g.CreatedBy == currentUserId && !g.IsDeleted)
                .Select(g => new GroupResponse
                {
                    Id = g.Id,
                    Title = g.Title,
                    CreatedAt = g.CreatedAt,
                    LastModifiedAt = g.LastModifiedAt,
                })
                .FirstOrDefaultAsync(cancellationToken);
            
            if (taskGroup == null)
            {
                return NotFound(new { message = "Группа задач не найдена" });
            }

            return Ok(taskGroup);
        }

        // PATCH: api/task/{id}/group/add/{groupId}
        [HttpPatch("{id}/group/add/{groupId}")]
        [Authorize]
        public async Task<IActionResult> AddToTaskGroup([FromRoute] Guid id, [FromRoute] Guid groupId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var group = await _context.TaskGroups
                .Where(g => g.Id == groupId && g.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (group == null)
            {
                return NotFound(new {message = "Группы задач с таким id не существует"});
            }

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
            {
                return NotFound(new {message = "Задача не найдена"});
            }

            if (task.TaskGroupId == groupId)
            {
                return BadRequest(new {message = "Нельзя добавить в ту же группу задач"});
            }

            // Если заметка была в другой группе, обновляем её timestamp
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

            return Ok( new
            {
                taskId = task.Id,
                taskGroupId = task.TaskGroupId,
                lastModifiedAt = task.LastModifiedAt,
            });
        }

        // PATCH: api/task/{id}/group/remove
        [HttpPatch("{id}/group/remove")]
        [Authorize]
        public async Task<IActionResult> RemoveFromTaskGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
            {
                return NotFound(new { message = "Задача не найдена" });
            }

            if (!task.TaskGroupId.HasValue)
            {
                return BadRequest(new { message = "Задача не находится в группе задач" });
            }

            // Обновляем timestamp старой группы
            var oldGroup = await _context.TaskGroups.FindAsync(task.TaskGroupId.Value);
            if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
            {
                oldGroup.LastModifiedAt = DateTime.UtcNow;
            }

            task.TaskGroupId = null;
            task.LastModifiedAt = DateTime.UtcNow;
    
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Задача {taskId} удалена из группы задач {oldGroupId}", 
                    task.Id, oldGroup?.Id ?? Guid.Empty);

            return Ok( new
            {
                taskId = task.Id,
                taskGroupId = task.TaskGroupId,
                lastModifiedAt = task.LastModifiedAt,
            });
        }

        // DELETE: api/task/group/delete/{groupId}
        [HttpDelete("group/delete/{groupId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTaskGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var taskGroup = await _context.TaskGroups
                .Where(t => t.Id == groupId && t.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (taskGroup == null)
            {
                return NotFound(new { message = "Группа задач не найдена" });
            }

            taskGroup.IsDeleted = true;
            taskGroup.DeletedAt = DateTime.UtcNow;
            taskGroup.LastModifiedAt = DateTime.UtcNow;

            // Сбрасываем NoteGroupId у всех заметок в группе и обновляем их timestamps
            var tasksInGroup = await _context.Tasks
                .Where(t => t.TaskGroupId == groupId && t.CreatedBy == currentUserId)
                .ToListAsync(cancellationToken);

            foreach (var task in tasksInGroup)
            {
                task.TaskGroupId = null;
                task.LastModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Группа {groupId} удалена (soft-delete), задачи ({count}) очищены", 
                    groupId, tasksInGroup.Count);

            return NoContent();
        }
    }
}