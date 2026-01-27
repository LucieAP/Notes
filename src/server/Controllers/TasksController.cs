using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Interfaces;

namespace server.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ILogger<TasksController> _logger;
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;

    public TasksController(
        ILogger<TasksController> logger,
        IUserService userService,
        ITaskService taskService)
    {
        _logger = logger;
        _userService = userService;
        _taskService = taskService;
    }

    // GET: api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetTaskResponse>>> GetTasks(CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.GetAllTasksAsync(cancellationToken);

        return Ok(tasks);
    } 

    // GET: api/tasks/id
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetTaskResponse>> GetTaskById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var task = await _taskService.GetTaskByIdAsync(id, currentUserId, cancellationToken);

        if (task == null)
        {
            return NotFound( new {message = "Задача не найдена"});
        }

        _logger.LogInformation("Найдена задача {TaskId}", task.Id);

        return Ok(task);
    }

    // POST: api/tasks/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var response = await _taskService.CreateTaskAsync(createTaskRequest, currentUserId, cancellationToken);

        return CreatedAtAction(
            nameof(GetTaskById),
            new {id = response.Id},
            response);
    }

    // PATCH: api/tasks/{id}/favorite
    [HttpPatch("{id}/favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.ToggleFavoriteAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/tasks/{id}/checkbox
    [HttpPatch("{id}/checkbox")]
    [Authorize]
    public async Task<IActionResult> ToggleCheckbox([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.ToggleCheckboxAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/tasks/{id}/
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateItemRequest updateItemRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.UpdateTaskAsync(id, currentUserId, updateItemRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // PATCH: api/tasks/{id}/color
    [HttpPatch("{id}/color")]
    [Authorize]
    public async Task<IActionResult> UpdateTaskColor([FromRoute] Guid id, [FromBody] UpdateColorRequest updateColorRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.UpdateColorAsync(id, currentUserId, updateColorRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // DELETE: api/tasks/delete/{id}
    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTaskById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.DeleteTaskByIdAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new {message = response.ErrorMessage});
        }

        return NoContent();
    }

    // PATCH: api/tasks/{id}/trash
    [HttpPatch("{id}/trash")]
    [Authorize]
    public async Task<IActionResult> TrashTask([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.TrashTaskAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // POST: api/tasks/group/create
    [HttpPost("group/create")]
    [Authorize]
    public async Task<IActionResult> CreateTaskGroup([FromBody]CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.CreateTaskGroupAsync(currentUserId, createGroupRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return CreatedAtAction(
            nameof(GetTaskGroup),
            new { id = response.Value?.Id },
            response.Value
        );
    }

    // GET: api/tasks/group/{id}
    [HttpGet("group/{id}")]
    [Authorize]
    public async Task<IActionResult> GetTaskGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var response = await _taskService.GetTaskGroupAsync(id, currentUserId, cancellationToken);

        if (response == null)
        {
            return NotFound(new { message = "Группа задач не найдена" });
        }

        return Ok(response);
    }

    // PATCH: api/tasks/{id}/group/add/{groupId}
    [HttpPatch("{id}/group/add/{groupId}")]
    [Authorize]
    public async Task<IActionResult> AddToTaskGroup([FromRoute] Guid id, [FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.AddToGroupAsync(id, groupId, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/tasks/{id}/group/remove
    [HttpPatch("{id}/group/remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFromTaskGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.RemoveFromGroupAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // DELETE: api/tasks/group/delete/{groupId}
    [HttpDelete("group/delete/{groupId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTaskGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _taskService.DeleteTaskGroupAsync(groupId, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return NoContent();
    }
}