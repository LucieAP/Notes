using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Interfaces;

namespace server.Controllers;

[Route("api/recipes")]
[ApiController]
public class RecipesController : Controller
{
    private readonly ILogger<RecipesController> _logger;
    private readonly IUserService _userService;
    private readonly IRecipeService _recipeService;

    public RecipesController(
        ILogger<RecipesController> logger, 
        IUserService userService,
        IRecipeService recipeService)
    {
        _logger = logger;
        _userService = userService;
        _recipeService = recipeService;
    }

    // GET: api/recipes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetRecipeResponse>>> GetRecipes(CancellationToken cancellationToken = default)
    {
        var recipes = await _recipeService.GetAllRecipesAsync(cancellationToken);

        return Ok(recipes);
    } 

    // GET: api/recipes/id
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetRecipeResponse>> GetRecipeById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var recipe = await _recipeService.GetRecipeByIdAsync(id, currentUserId, cancellationToken);
        
        if (recipe == null)
        {
            return NotFound( new {message = "Рецепт не найден"});
        }

        _logger.LogInformation("Найден рецепт {recipeId}", recipe.Id);

        return Ok(recipe);
    }
    
    // POST: api/recipes/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeRequest createRecipeRequest, CancellationToken cancellationToken = default)
    {

        var currentUserId = _userService.GetUserId(User);

        var response = await _recipeService.CreateRecipeAsync(createRecipeRequest, currentUserId, cancellationToken);

        return CreatedAtAction(
            nameof(GetRecipeById),
            new {id = response.Id},
            response);
    }

    // PATCH: api/recipes/{id}/favorite
    [HttpPatch("{id}/favorite")]
    [Authorize]
    public async Task<IActionResult> TogglePin([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.ToggleFavoriteAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/recipes/{id}/
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateRecipe([FromRoute] Guid id, [FromBody] UpdateItemRequest updateItemRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.UpdateRecipeAsync(id, currentUserId, updateItemRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // DELETE: api/recipes/delete/{id}
    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRecipeById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.DeleteRecipeByIdAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new {message = response.ErrorMessage});
        }

        return NoContent();       
    }

    // PATCH: api/recipes/{id}/trash
    [HttpPatch("{id}/trash")]
    [Authorize]
    public async Task<IActionResult> TrashRecipe([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.TrashRecipeAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // Группа рецептов

    // POST: api/recipes/group/create
    [HttpPost("group/create")]
    [Authorize]
    public async Task<IActionResult> CreateRecipeGroup([FromBody]CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.CreateRecipeGroupAsync(currentUserId, createGroupRequest, cancellationToken);     

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return CreatedAtAction(
            nameof(GetRecipeGroup),
            new { id = response.Value?.Id },
            response.Value
        );
    }

    // GET: api/recipes/group/{groupId}
    [HttpGet("group/{groupId}")]
    [Authorize]
    public async Task<IActionResult> GetRecipeGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var response = await _recipeService.GetRecipeGroupByIdAsync(groupId, currentUserId, cancellationToken);

        if (response == null)
        {
            return NotFound(new { message = "Группа не найдена" });
        }

        return Ok(response);
    }

    // PATCH: api/recipes/{id}/group/add/{groupId}
    [HttpPatch("{id}/group/add/{groupId}")]
    [Authorize]
    public async Task<IActionResult> AddToGroup([FromRoute] Guid id, [FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.AddToGroupAsync(id, groupId, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // PATCH: api/recipes/{id}/group/remove
    [HttpPatch("{id}/group/remove")]
    [Authorize]
    public async Task<IActionResult> RemoveFromGroup([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.RemoveFromGroupAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return Ok(response.Value);
    }

    // DELETE: api/recipes/group/delete/{groupId}
    [HttpDelete("group/delete/{groupId}")]
    [Authorize]
    public async Task<IActionResult> DeleteRecipeGroup([FromRoute] Guid groupId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.DeleteRecipeGroupAsync(groupId, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage});
        }

        return NoContent();
    }

    // ********************************************** /

    // Ингредиенты рецепта

    // GET: api/recipes/ingredient/id
    [HttpGet("ingredient/{id}")]
    [Authorize]
    public async Task<ActionResult<GetIngredientResponse>> GetIngredientById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var ingredient = await _recipeService.GetIngredientByIdAsync(id, currentUserId, cancellationToken);
        
        if (ingredient == null)
        {
            return NotFound( new {message = "Ингредиент не найден"});
        }

        _logger.LogInformation("Найден ингредиент {ingredientId}", ingredient.Id);

        return Ok(ingredient);
    }

    // POST: api/recipes/ingredient/create
    [HttpPost("ingredient/create")]
    [Authorize]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest createIngredientRequest, CancellationToken cancellationToken = default)
    {

        var currentUserId = _userService.GetUserId(User);

        var response = await _recipeService.CreateIngredientAsync(createIngredientRequest, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return CreatedAtAction(
            nameof(GetIngredientById),
            new {id = response.Value.Id},
            response.Value);
    }

    // PATCH: api/recipes/ingredient/{id}/
    [HttpPatch("ingredient/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateIngredient([FromRoute] Guid id, [FromBody] UpdateIngredientRequest updateIngredientRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.UpdateIngredientAsync(id, currentUserId, updateIngredientRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // DELETE: api/recipes/ingredient/delete/{id}
    [HttpDelete("ingredient/delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteIngredientById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.DeleteIngredientByIdAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new {message = response.ErrorMessage});
        }

        return NoContent();       
    }

    // Группа ингредиентов

    // GET: api/recipes/ingredient/group/{id}
    [HttpGet("ingredient/group/{id}")]
    [Authorize]
    public async Task<ActionResult<GetIngredientGroupResponse>> GetIngredientGroupById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var group = await _recipeService.GetIngredientGroupByIdAsync(id, currentUserId, cancellationToken);
        
        if (group == null)
        {
            return NotFound( new {message = "Группа ингредиентов не найдена"});
        }

        _logger.LogInformation("Найдена группа ингредиентов {groupId}", group.Id);

        return Ok(group);
    }

    // POST: api/recipes/ingredient/group/create
    [HttpPost("ingredient/group/create")]
    [Authorize]
    public async Task<IActionResult> CreateIngredientGroup([FromBody] CreateIngredientGroupRequest createIngredientGroupRequest, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var response = await _recipeService.CreateIngredientGroupAsync(createIngredientGroupRequest, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return CreatedAtAction(
            nameof(GetIngredientGroupById),
            new {id = response.Value.Id},
            response.Value);
    }

    // PATCH: api/recipes/ingredient/group/{id}/
    [HttpPatch("ingredient/group/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateIngredientGroup([FromRoute] Guid id, [FromBody] UpdateItemRequest updateItemRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.UpdateIngredientGroupAsync(id, currentUserId, updateItemRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }


    // DELETE: api/recipes/ingredient/group/delete/{id}
    [HttpDelete("ingredient/group/delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteIngredientGroupById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.DeleteIngredientGroupByIdAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new {message = response.ErrorMessage});
        }

        return NoContent();       
    }

    // GET: api/recipes/step/{id}
    [HttpGet("step/{id}")]
    [Authorize]
    public async Task<ActionResult<GetRecipeStepResponse>> GetRecipeStepById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        var step = await _recipeService.GetRecipeStepAsync(id, currentUserId, cancellationToken);
        
        if (step == null)
        {
            return NotFound( new {message = "Шаг рецепта не найден"});
        }

        _logger.LogInformation("Найден шаг рецепта {stepId}", step.Id);

        return Ok(step);
    }

    // POST: api/recipes/step/create
    [HttpPost("step/create")]
    [Authorize]
    public async Task<IActionResult> CreateRecipeStep([FromBody] CreateRecipeStepRequest createRecipeStepRequest, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);

        try
        {
            var response = await _recipeService.CreateRecipeStepAsync(createRecipeStepRequest, currentUserId, cancellationToken);

            return CreatedAtAction(
                nameof(GetRecipeStepById),
                new {id = response.Id},
                response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PATCH: api/recipes/step/{id}
    [HttpPatch("step/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateRecipeStep([FromRoute] Guid id, [FromBody] UpdateItemRequest updateItemRequest , CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.UpdateRecipeStepAsync(id, currentUserId, updateItemRequest, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new { message = response.ErrorMessage });
        }

        return Ok(response.Value);
    }

    // DELETE: api/recipes/step/delete/{id}
    [HttpDelete("step/delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRecipeStepById ([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userService.GetUserId(User);
        var response = await _recipeService.DeleteRecipeStepByIdAsync(id, currentUserId, cancellationToken);

        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode ?? 500, new {message = response.ErrorMessage});
        }

        return NoContent();       
    }
}