using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Interfaces;

namespace server.Controllers;

[Route("api/notes")]
[ApiController]
public class NotesController : ControllerBase

{
    private readonly ILogger<NotesController> _logger;
    private readonly IUserService _userService;
    private readonly INoteService _noteService;

    public NotesController(
        ILogger<NotesController> logger, 
        IUserService userService,
        INoteService noteService)
    {
        _logger = logger;
        _userService = userService;
        _noteService = noteService;
    }

    // GET: api/notes
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GetNoteResponse>>> GetNotes(CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var notes = await _noteService.GetAllNotesAsync(currentUserId, cancellationToken);

        return Ok(notes);
    } 

    // GET: api/notes/id
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetNoteResponse>> GetNoteById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var note = await _noteService.GetNoteByIdAsync(id, currentUserId, cancellationToken);
        
        if (note == null)
        {
            return NotFound( new {message = "Заметка не найдена"});
        }

        _logger.LogInformation("Найдена заметка {NoteId}", note.Id);

        return Ok(note);
    }
    
    // POST: api/notes/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest createNoteRequest, CancellationToken cancellationToken = default)
    {

        var currentUserId = _userService.GetUserId(User);

        try
        {
            var response = await _noteService.CreateNoteAsync(createNoteRequest, currentUserId, cancellationToken);

            return CreatedAtAction(
                nameof(GetNoteById),
                new {id = response.Id},
                response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Ошибка при создании заметки. Пользователь {UserId}. Тело запроса {@Request}",
                currentUserId,
                createNoteRequest);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PATCH: api/notes/{id}/pin
    [HttpPatch("{id}/pin")]
    [Authorize]
    public async Task<IActionResult> TogglePin([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.TogglePinAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // GET: api/notes/pinned
    [HttpGet("pinned")]
    [Authorize]
    public async Task<IActionResult> GetPinnedNotes(CancellationToken cancellationToken)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.GetPinnedNotesAsync(currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value!.Notes);
    }

    // PATCH: api/notes/{id}/
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] UpdateNoteRequest updateNoteRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.UpdateNoteAsync(id, currentUserId, updateNoteRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // PATCH: api/notes/{id}/color
    [HttpPatch("{id}/color")]
    [Authorize]
    public async Task<IActionResult> UpdateNoteColor([FromRoute] Guid id, [FromBody] UpdateColorRequest updateColorRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.UpdateColorAsync(id, currentUserId, updateColorRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // DELETE: api/notes/delete/{id}
    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteNoteById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.DeleteNoteByIdAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new {message = response.ErrorMessage});
        }

        return NoContent();       
    }

    // PATCH: api/notes/{id}/trash
    [HttpPatch("{id}/trash")]
    [Authorize]
    public async Task<IActionResult> TrashNote([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.TrashNoteAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/notes/{id}/restore
    [HttpPatch("{id}/restore")]
    [Authorize]
    public async Task<IActionResult> RestoreNote([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.RestoreNoteAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // GET: api/notes/trashed
    [HttpGet("trashed")]
    [Authorize]
    public async Task<IActionResult> GetTrashedNotes(CancellationToken cancellationToken)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.GetTrashedNotesAsync(currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value!.Notes);
    }

    // POST: api/notes/group/create
    [HttpPost("group/create")]
    [Authorize]
    public async Task<IActionResult> CreateNoteGroup([FromBody]CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.CreateNoteGroupAsync(currentUserId, createGroupRequest, cancellationToken);     

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return CreatedAtAction(
            nameof(GetNoteGroup),
            new { id = response.Value?.Id },
            response.Value
        );
    }

    // GET: api/notes/group/{groupId}
    [HttpGet("group/{groupId}")]
    [Authorize]
    public async Task<IActionResult> GetNoteGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var response = await _noteService.GetNoteGroupAsync(groupId, currentUserId, cancellationToken);

        if (response == null)
        {
            return NotFound(new { message = "Группа не найдена" });
        }

        return Ok(response);
    }

    // PATCH: api/notes/{id}/group/add/{groupId}
    [HttpPatch("{id}/group/add/{groupId}")]
    [Authorize]
    public async Task<IActionResult> AddToGroup([FromRoute] Guid id, [FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.AddToGroupAsync(id, groupId, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/notes/{id}/group/remove
    [HttpPatch("{id}/group/remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFromGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.RemoveFromGroupAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // DELETE: api/notes/group/delete/{groupId}
    [HttpDelete("group/delete/{groupId}")]
    [Authorize]
    public async Task<IActionResult> DeleteNoteGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _noteService.DeleteNoteGroupAsync(groupId, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return NoContent();
    }  
}