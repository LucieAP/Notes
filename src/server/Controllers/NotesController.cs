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

            return Ok(new{
                id = note.Id,
                isPinned = note.IsPinned,
                lastModifiedAt = note.LastModifiedAt
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
    }
}