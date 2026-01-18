using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace server.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NotesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotesController> _logger;
        private readonly UserService _userService;

        public NotesController(AppDbContext context, ILogger<NotesController> logger, UserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        // GET: api/notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetNoteResponse>>> GetNotes(CancellationToken cancellationToken = default)
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
                    NoteGroup = n.NoteGroup != null ? new GetNoteGroup
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

            return Ok(notes);
        } 

        // GET: api/notes/id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetNoteResponse>> GetNote([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var note = await _context.Notes
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.NoteGroup)
                .Where(n => n.Id == id && n.CreatedBy == currentUserId)
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
                    NoteGroup = n.NoteGroup != null ? new GetNoteGroup
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
            
            if (note == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Вывод всех найденных заметок");

            return Ok(note);
        }
        
        // POST: api/notes/create
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest createNoteRequest, CancellationToken cancellationToken = default)
        {

            var currentUserId = _userService.GetUserId(User);

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

            var response = new CreateNoteResponse
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

            _logger.LogInformation("Создана новая заметка {NoteId}", note.Id);

            return CreatedAtAction(
                nameof(GetNote),
                new {id = note.Id},
                response);
        }

        // PATCH: api/notes/{id}/pin
        [HttpPatch("{id}/pin")]
        [Authorize]
        public async Task<IActionResult> ToggleNotePinStatus([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var note = await _context.Notes
                .Where(n => n.Id == id && n.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (note == null)
            {
                return NotFound();
            }

            note.IsPinned = !note.IsPinned;
            note.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Кнопка закрепления заметки {NoteId} переключена на {IsPinned}", note.Id, note.IsPinned);

            return Ok(new{
                id = note.Id,
                isPinned = note.IsPinned,
                lastModifiedAt = note.LastModifiedAt
            });
        }

        // PATCH: api/notes/{id}/
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] UpdateNoteRequest updateNoteRequest , CancellationToken cancellationToken = default)
        {
            if (updateNoteRequest.Title == null && 
                updateNoteRequest.Description == null && 
                updateNoteRequest.BackgroundColor == null)
            {
                return BadRequest(new { message = "Ни одного параметра не было передано" });
            }

            var currentUserId = _userService.GetUserId(User);

            var note = await _context.Notes
                .Where(n => n.Id == id && n.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (note == null)
            {
                return NotFound();
            }

            var wasUpdated = false;

            if (updateNoteRequest.Title != null)
            {
                var trimmedTitle = updateNoteRequest.Title.Trim();  // Удаляем пробелы с начала и конца строки
        
                if (string.IsNullOrWhiteSpace(trimmedTitle))        // Проверяем пустоту строки
                {
                    return BadRequest(new { message = "Название не может быть пустым" });
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
            if (updateNoteRequest.Description != null)
            {
                var trimmedDescription = string.IsNullOrWhiteSpace(updateNoteRequest.Description)  // Проверяем пустоту строки
                    ? null 
                    : updateNoteRequest.Description.Trim();     // Удаляем пробелы с начала и конца строки
                
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
            if (updateNoteRequest.BackgroundColor.HasValue && 
                updateNoteRequest.BackgroundColor.Value != note.BackgroundColor)
            {
                note.BackgroundColor = updateNoteRequest.BackgroundColor.Value;
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

            return Ok(new{
                id = note.Id,
                title = note.Title,
                description = note.Description,
                backgroundColor = note.BackgroundColor,
                lastModifiedAt = note.LastModifiedAt,
                wasUpdated = wasUpdated
            });
        }

        // DELETE: api/notes/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNoteById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var note = await _context.Notes
                .Where(n => n.Id == id && n.CreatedBy == currentUserId && n.IsDeleted != true)
                .FirstOrDefaultAsync(cancellationToken);

            if (note == null)
            {
                return NotFound();
            }

            note.LastModifiedAt = DateTime.UtcNow;

            // Мягкое удаление
            note.IsDeleted = true;
            note.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Note {NotesId} была удалена пользователем {Userid}", id, currentUserId);

            return NoContent();       
        }

        // POST: api/notes/group/create
        [HttpPost("group/create")]
        [Authorize]
        public async Task<IActionResult> CreateNoteGroup([FromBody]CreateNoteGroupRequest createNoteGroupRequest, CancellationToken cancellationToken = default)
        {

            var currentUserId = _userService.GetUserId(User);
            var trimmedTitle = createNoteGroupRequest.Title.Trim();

            if (string.IsNullOrEmpty(trimmedTitle))
            {
                return BadRequest(new { message = "Название группы не может быть пустым" });
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

            var response = new CreateNoteGroupResponse
            {
                Id = noteGroup.Id,
                Title = noteGroup.Title,
                CreatedAt = noteGroup.CreatedAt,
                LastModifiedAt = noteGroup.LastModifiedAt
            };

            return CreatedAtAction(
                nameof(GetNoteGroup),
                new { id = noteGroup.Id },
                response
            );
        }

        // GET: api/notes/group/{id}
        [HttpGet("group/{id}")]
        [Authorize]
        public async Task<IActionResult> GetNoteGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var noteGroup = await _context.NoteGroups
                .AsNoTracking()
                .Where(g => g.Id == id && g.CreatedBy == currentUserId && !g.IsDeleted)
                .Select(g => new GetNoteGroup
                {
                    Id = g.Id,
                    Title = g.Title,
                    CreatedAt = g.CreatedAt,
                    LastModifiedAt = g.LastModifiedAt,
                })
                .FirstOrDefaultAsync(cancellationToken);
            
            if (noteGroup == null)
            {
                return NotFound(new { message = "Группа не найдена" });
            }

            return Ok(noteGroup);
        }

        // PATCH: api/notes/{id}/group/add/{groupId}
        [HttpPatch("{id}/group/add/{groupId}")]
        [Authorize]
        public async Task<IActionResult> AddToGroup([FromRoute] Guid id, [FromRoute] Guid groupId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var group = await _context.NoteGroups
                .Where(g => g.Id == groupId && g.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (group == null)
            {
                return NotFound(new {message = "Группы с таким id не существует"});
            }

            var note = await _context.Notes
                .Where(n => n.Id == id && n.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (note == null)
            {
                return NotFound();
            }

            if (note.NoteGroupId == groupId)
            {
                return BadRequest(new {message = "Нельзя добавить в ту же группу"});
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

            return Ok( new
            {
                noteId = note.Id,
                noteGroupId = note.NoteGroupId,
                lastModifiedAt = note.LastModifiedAt,
            });
        }

        // PATCH: api/notes/{id}/group/remove
        [HttpPatch("{id}/group/remove")]
        [Authorize]
        public async Task<IActionResult> RemoveFromGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var note = await _context.Notes
                .Where(n => n.Id == id && n.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (note == null)
            {
                return NotFound(new { message = "Заметка не найдена" });
            }

            if (!note.NoteGroupId.HasValue)
            {
                return BadRequest(new { message = "Заметка не находится в группе" });
            }

            // Обновляем timestamp старой группы
            var oldGroup = await _context.NoteGroups.FindAsync(note.NoteGroupId.Value);
            if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
            {
                oldGroup.LastModifiedAt = DateTime.UtcNow;
            }

            note.NoteGroupId = null;
            note.LastModifiedAt = DateTime.UtcNow;
    
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Заметка {noteId} удалена из группы {oldGroupId}", 
                    note.Id, oldGroup?.Id ?? Guid.Empty);

            return Ok( new
            {
                noteId = note.Id,
                noteGroupId = note.NoteGroupId,
                lastModifiedAt = note.LastModifiedAt,
            });
        }

        // DELETE: api/notes/group/delete/{groupId}
        [HttpDelete("group/delete/{groupId}")]
        [Authorize]
        public async Task<IActionResult> DeleteNoteGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            var noteGroup = await _context.NoteGroups
                .Where(n => n.Id == groupId && n.CreatedBy == currentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (noteGroup == null)
            {
                return NotFound(new { message = "Группа не найдена" });
            }

            noteGroup.IsDeleted = true;
            noteGroup.DeletedAt = DateTime.UtcNow;
            noteGroup.LastModifiedAt = DateTime.UtcNow;

            // Сбрасываем NoteGroupId у всех заметок в группе и обновляем их timestamps
            var notesInGroup = await _context.Notes
                .Where(n => n.NoteGroupId == groupId && n.CreatedBy== currentUserId)
                .ToListAsync(cancellationToken);

            foreach (var note in notesInGroup)
            {
                note.NoteGroupId = null;
                note.LastModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Группа {groupId} удалена (soft-delete), заметки ({count}) очищены", 
                    groupId, notesInGroup.Count);

            return NoContent();
        }  
    }
}