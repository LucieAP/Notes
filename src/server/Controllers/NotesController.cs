using Microsoft.AspNetCore.Identity;
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

        public NotesController(AppDbContext context, ILogger<NotesController> logger)
        {
            _context = context;
            _logger = logger;
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
                    NoteGroup = new GetNoteGroup
                    {
                        Id = n.NoteGroup.Id,
                        Title = n.NoteGroup.Title,
                        CreatedAt = n.NoteGroup.CreatedAt,
                        LastModifiedAt = n.NoteGroup.LastModifiedAt,
                    },
                    CreatedBy = new GetUserResponse
                    {
                        Id = n.User.Id,
                        Name = n.User.Name,
                        Email = n.User.Email,
                        Picture = n.User.Picture
                    }
                })
                .ToListAsync(cancellationToken);

            if (notes == null)
            {
                return NotFound();  
            } 

            return Ok(notes);
        } 

        // GET: api/notes/id
        [HttpGet("{id}")]
        public async Task<ActionResult<GetNoteResponse>> GetNote([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var note = await _context.Notes
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.NoteGroup)
                .Where(n => n.Id == id)
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
                    NoteGroup = new GetNoteGroup
                    {
                        Id = n.NoteGroup.Id,
                        Title = n.NoteGroup.Title,
                        CreatedAt = n.NoteGroup.CreatedAt,
                        LastModifiedAt = n.NoteGroup.LastModifiedAt,
                    },
                    CreatedBy = new GetUserResponse
                    {
                        Id = n.User.Id,
                        Name = n.User.Name,
                        Email = n.User.Email,
                        Picture = n.User.Picture
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);
            
            if (note == null)
            {
                return NotFound();
            }

            return Ok(note);
        }
    }
}