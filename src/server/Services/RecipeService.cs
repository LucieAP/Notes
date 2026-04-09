using Microsoft.EntityFrameworkCore;
using server.Interfaces;
using server.Services.Common;

public class RecipeService : IRecipeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(AppDbContext context, ILogger<RecipeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GetRecipeResponse>> GetAllRecipesAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipes = await _context.Recipes
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.RecipeGroup)
            .Where(r => r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .Select(r => new GetRecipeResponse
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                IsFavorite = r.IsFavorite,
                CreatedAt = r.CreatedAt,
                LastModifiedAt = r.LastModifiedAt,
                IsTrashed = r.IsTrashed,
                IsDeleted = r.IsDeleted,
                DeletedAt = r.DeletedAt,
                RecipeGroup = r.RecipeGroup != null ? new GroupResponse
                {
                    Id = r.RecipeGroup.Id,
                    Title = r.RecipeGroup.Title,
                    CreatedAt = r.RecipeGroup.CreatedAt,
                    LastModifiedAt = r.RecipeGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = r.User.Id,
                    Name = r.User.Name,
                    Email = r.User.Email,
                    Picture = r.User.Picture,
                    EmailVerified = r.User.EmailVerified,
                    LastLoginAt = r.User.LastLoginAt
                },
                Ingredients = r.Ingredients
                    .OrderBy(i => i.Position)
                    .ThenBy(i => i.CreatedAt)
                    .Select(i => new GetIngredientResponse
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Note = i.Note,
                        Position = i.Position,
                        CreatedAt = i.CreatedAt,
                        IngredientGroupId = i.IngredientGroupId,
                        RecipeId = i.RecipeId
                    })
                    .ToList(),
                Steps = r.RecipeSteps
                    .OrderBy(rs => rs.Position)
                    .ThenBy(rs => rs.CreatedAt)
                    .Select(rs => new GetRecipeStepResponse {
                        Id = rs.Id,
                        Description = rs.Description,
                        Position = rs.Position,
                        CreatedAt = rs.CreatedAt,
                        RecipeId = rs.RecipeId
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Вывод всех найденных рецептов");
        
        return recipes;
    }

    public async Task<GetRecipeResponse?> GetRecipeByIdAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.RecipeGroup)
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .Select(r => new GetRecipeResponse
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                IsFavorite = r.IsFavorite,
                CreatedAt = r.CreatedAt,
                LastModifiedAt = r.LastModifiedAt,
                IsTrashed = r.IsTrashed,
                IsDeleted = r.IsDeleted,
                DeletedAt = r.DeletedAt,
                RecipeGroup = r.RecipeGroup != null ? new GroupResponse
                {
                    Id = r.RecipeGroup.Id,
                    Title = r.RecipeGroup.Title,
                    CreatedAt = r.RecipeGroup.CreatedAt,
                    LastModifiedAt = r.RecipeGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = r.User.Id,
                    Name = r.User.Name,
                    Email = r.User.Email,
                    Picture = r.User.Picture,
                    EmailVerified = r.User.EmailVerified,
                    LastLoginAt = r.User.LastLoginAt
                },
                Ingredients = r.Ingredients
                    .OrderBy(i => i.Position)
                    .ThenBy(i => i.CreatedAt)
                    .Select(i => new GetIngredientResponse
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Note = i.Note,
                        Position = i.Position,
                        CreatedAt = i.CreatedAt,
                        IngredientGroupId = i.IngredientGroupId,
                        RecipeId = i.RecipeId
                    })
                    .ToList(),
                Steps = r.RecipeSteps
                    .OrderBy(rs => rs.Position)
                    .ThenBy(rs => rs.CreatedAt)
                    .Select(rs => new GetRecipeStepResponse {
                        Id = rs.Id,
                        Description = rs.Description,
                        Position = rs.Position,
                        CreatedAt = rs.CreatedAt,
                        RecipeId = rs.RecipeId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return recipe;
    }

    public async Task<CreateRecipeResponse> CreateRecipeAsync(CreateRecipeRequest createRecipeRequest, Guid currentUserId, CancellationToken cancellationToken)
    {
        if (createRecipeRequest.RecipeGroupId != null)
        {
            var groupExists = await _context.RecipeGroups
                .AnyAsync(
                    rg => rg.Id == createRecipeRequest.RecipeGroupId
                          && rg.CreatedBy == currentUserId
                          && !rg.IsDeleted,
                    cancellationToken);

            if (!groupExists)
            {
                throw new InvalidOperationException("Группа не найдена");
            }
        }

        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = createRecipeRequest.Title,
            Description = createRecipeRequest.Description,
            IsFavorite = createRecipeRequest.IsFavorite,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsTrashed = false,
            IsDeleted = false,
            DeletedAt = null,
            CreatedBy = currentUserId,
            RecipeGroupId = createRecipeRequest.RecipeGroupId
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Создан новый рецепт c идентификатором {recipeId}", recipe.Id);

        return new CreateRecipeResponse
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            IsFavorite = recipe.IsFavorite,
            CreatedAt = recipe.CreatedAt,
            LastModifiedAt = recipe.LastModifiedAt,
            IsTrashed = recipe.IsTrashed,
            IsDeleted = recipe.IsDeleted,
            DeletedAt = recipe.DeletedAt,
            CreatedBy = recipe.CreatedBy,
            RecipeGroupId = recipe.RecipeGroupId
        };
    } 
    public async Task<OperationResult<ToggleFavoriteResponse?>> ToggleFavoriteAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<ToggleFavoriteResponse?>.Failure("Рецепт не найден", 404);
        }

        recipe.IsFavorite = !recipe.IsFavorite;
        recipe.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Кнопка избранное рецепта {recipeId} переключена на {isFavorite}", recipe.Id, recipe.IsFavorite);

        return OperationResult<ToggleFavoriteResponse?>.Success(new ToggleFavoriteResponse
        {
            Id = recipe.Id,
            IsFavorite = recipe.IsFavorite,
            LastModifiedAt = recipe.LastModifiedAt
        });
    }

    public async Task<OperationResult<UpdateItemResponse>> UpdateRecipeAsync(Guid recipeId, Guid currentUserId, UpdateItemRequest updateItemRequest, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<UpdateItemResponse>.Failure("Рецепт не найден", 404);
        }

        var response = UpdateItemHelper.ApplyRecipeUpdate(recipe, currentUserId, updateItemRequest, _logger);

        if (response.IsSuccess && response.Value.WasUpdated)
        {
            await _context.SaveChangesAsync(cancellationToken); 
        }

        return response;
    }

    public async Task<OperationResult> DeleteRecipeByIdAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult.Failure("Рецепт не найден или уже удален", 404);
        }

        var response = SoftDeleteHelper.SoftDelete(recipe, "Recipe", currentUserId, _logger);

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken); 
        }

        return OperationResult.Success();
    }

    public async Task<OperationResult<TrashResponse>> TrashRecipeByIdAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
                .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
                
        if (recipe == null)
        {
            return OperationResult<TrashResponse>.Failure("Рецепт не найден", 404);
        }

        recipe.IsTrashed = !recipe.IsTrashed;
        recipe.LastModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Рецепт {RecipeId} перемещен в корзину пользователем {UserId}", recipe.Id, currentUserId);
    
        return OperationResult<TrashResponse>.Success( new TrashResponse
        {
            Id = recipe.Id,
            IsTrashed = recipe.IsTrashed,
            LastModifiedAt = recipe.LastModifiedAt
        });
    }

    public async Task<OperationResult<TrashResponse>> RestoreRecipeAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
                .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && r.IsTrashed)
                .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<TrashResponse>.Failure("Рецепт не найден или уже восстановлен", 404);
        }

        recipe.IsTrashed = false;
        recipe.LastModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Рецепт {RecipeId} восстановлен пользователем {UserId}", recipe.Id, currentUserId);

        return OperationResult<TrashResponse>.Success(new TrashResponse
        {
            Id = recipe.Id,
            IsTrashed = recipe.IsTrashed,
            LastModifiedAt = recipe.LastModifiedAt
        });
    }

    public async Task<OperationResult<TrashedRecipesResponse>> GetTrashedRecipesAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var trashedRecipes = await _context.Recipes
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.RecipeGroup)
            .Where(r => r.IsTrashed && r.CreatedBy == currentUserId && !r.IsDeleted)
            .OrderByDescending(r => r.LastModifiedAt)
            .Select(r => new GetRecipeResponse
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                IsFavorite = r.IsFavorite,
                CreatedAt = r.CreatedAt,
                LastModifiedAt = r.LastModifiedAt,
                IsTrashed = r.IsTrashed,
                IsDeleted = r.IsDeleted,
                DeletedAt = r.DeletedAt,
                RecipeGroup = r.RecipeGroup != null ? new GroupResponse
                {
                    Id = r.RecipeGroup.Id,
                    Title = r.RecipeGroup.Title,
                    CreatedAt = r.RecipeGroup.CreatedAt,
                    LastModifiedAt = r.RecipeGroup.LastModifiedAt,
                } : null,
                CreatedBy = new GetUserResponse
                {
                    Id = r.User.Id,
                    Name = r.User.Name,
                    Email = r.User.Email,
                    Picture = r.User.Picture,
                    EmailVerified = r.User.EmailVerified,
                    LastLoginAt = r.User.LastLoginAt
                }
            })
            .ToListAsync(cancellationToken);

        return OperationResult<TrashedRecipesResponse>.Success(new TrashedRecipesResponse
        {
            Recipes = trashedRecipes
        });
    }

    // Группа рецептов

    public async Task<OperationResult<CreateGroupResponse>> CreateRecipeGroupAsync(Guid currentUserId, CreateGroupRequest createGroupRequest, CancellationToken cancellationToken = default)
    {
        var trimmedTitle = createGroupRequest.Title.Trim();

        if (string.IsNullOrEmpty(trimmedTitle))
        {
            return OperationResult<CreateGroupResponse>.Failure("Название группы не может быть пустым", 400);
        }
        
        var RecipeGroup = new RecipeGroup
        {
            Id = Guid.NewGuid(),
            Title = trimmedTitle,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsDeleted = false,
            CreatedBy = currentUserId
        };

        _context.RecipeGroups.Add(RecipeGroup);
        
        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult<CreateGroupResponse>.Success(new CreateGroupResponse
        {
            Id = RecipeGroup.Id,
            Title = RecipeGroup.Title,
            CreatedAt = RecipeGroup.CreatedAt,
            LastModifiedAt = RecipeGroup.LastModifiedAt
        });
    }
    
    public async Task<GroupResponse?> GetRecipeGroupByIdAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipeGroup = await _context.RecipeGroups
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
        
        return recipeGroup;
    }

    public async Task<OperationResult<AddToGroupResponse>> AddToGroupAsync(Guid recipeId, Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var group = await _context.RecipeGroups
            .Where(g => g.Id == groupId && g.CreatedBy == currentUserId && !g.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (group == null)
        {
            return OperationResult<AddToGroupResponse>.Failure("Группы с таким id не существует", 404);
        }

        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<AddToGroupResponse>.Failure("Рецепт не найден", 404);
        }

        if (recipe.RecipeGroupId == groupId)
        {
            return OperationResult<AddToGroupResponse>.Failure("Нельзя добавить в ту же группу", 400);
        }

        // Если рецепт был в другой группе, обновляем её timestamp
        RecipeGroup? oldGroup = null;

        if (recipe.RecipeGroupId.HasValue)
        {
            oldGroup = await _context.RecipeGroups.FindAsync(recipe.RecipeGroupId.Value, cancellationToken);
            if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
            {
                oldGroup.LastModifiedAt = DateTime.UtcNow;
            }
        }

        recipe.RecipeGroupId = groupId;
        recipe.LastModifiedAt = DateTime.UtcNow;
        group.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Рецепт {recipeId} добавлен в группу {groupId} (старая группа: {oldGroupId})", 
                recipe.Id, groupId, oldGroup?.Id ?? Guid.Empty);

        return OperationResult<AddToGroupResponse>.Success( new AddToGroupResponse
        {
            ItemId = recipe.Id,
            ItemGroupId = recipe.RecipeGroupId.Value,
            LastModifiedAt = recipe.LastModifiedAt,
        });
    }

    public async Task<OperationResult<RemoveFromGroupResponse>> RemoveFromGroupAsync(Guid recipeId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<RemoveFromGroupResponse>.Failure("Рецепт не найден", 404);
        }

        if (!recipe.RecipeGroupId.HasValue)
        {
            return OperationResult<RemoveFromGroupResponse>.Failure("Рецепт не находится в группе", 400);
        }

        // Обновляем timestamp старой группы
        var oldGroup = await _context.RecipeGroups.FindAsync(recipe.RecipeGroupId.Value, cancellationToken);
        if (oldGroup != null && oldGroup.CreatedBy == currentUserId)
        {
            oldGroup.LastModifiedAt = DateTime.UtcNow;
        }

        var oldGroupId = oldGroup?.Id ?? recipe.RecipeGroupId.Value; // Сохраняем ID группы до обнуления

        recipe.RecipeGroupId = null;
        recipe.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Рецепт {recipeId} удалена из группы {oldGroupId}",
                recipe.Id, oldGroupId);

        return OperationResult<RemoveFromGroupResponse>.Success( new RemoveFromGroupResponse
        {
            ItemId = recipe.Id,
            ItemGroupId = oldGroupId,
            LastModifiedAt = recipe.LastModifiedAt,
        });
    }

    public async Task<OperationResult> DeleteRecipeGroupByIdAsync(Guid groupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var RecipeGroup = await _context.RecipeGroups
            .Where(r => r.Id == groupId && r.CreatedBy == currentUserId && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (RecipeGroup == null)
        {
            return OperationResult.Failure("Группа не найдена", 404);
        }

        var response = SoftDeleteHelper.SoftDelete(RecipeGroup, "Recipe Group", currentUserId, _logger);

        // Сбрасываем RecipeGroupId у всех заметок в группе и обновляем их timestamps
        var recipesInGroup = await _context.Recipes
            .Where(r => r.RecipeGroupId == groupId && r.CreatedBy == currentUserId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var recipe in recipesInGroup)
        {
            recipe.RecipeGroupId = null;
            recipe.LastModifiedAt = DateTime.UtcNow;
        }

        if (response.IsSuccess)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Группа {groupId} удалена (soft-delete), заметки ({count}) очищены", 
                groupId, recipesInGroup.Count);

        return OperationResult.Success();
    } 

    // Ингредиенты рецепта

    public async Task<OperationResult<CreateIngredientResponse>> CreateIngredientAsync(Guid recipeId, CreateIngredientRequest createIngredientRequest, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var trimmedName = createIngredientRequest.Name?.Trim();
        var trimmedNote = createIngredientRequest.Note?.Trim();

        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<CreateIngredientResponse>.Failure("Рецепт не найден", 404);
        }

        if (createIngredientRequest.IngredientGroupId != null)
        {
            var groupExists = await _context.IngredientGroups
                .Include(ig => ig.Recipe)
                .AnyAsync(
                    ig => ig.Id == createIngredientRequest.IngredientGroupId
                          && ig.RecipeId == recipeId
                          && ig.Recipe.CreatedBy == currentUserId
                          && !ig.Recipe.IsDeleted,
                    cancellationToken);

            if (!groupExists)
            {
                return OperationResult<CreateIngredientResponse>.Failure("Группа ингредиентов не найдена", 404);
            }
        }

        var nextPosition = await _context.Ingredients
            .Where(i => i.RecipeId == recipeId)
            .Select(i => (int?)i.Position)
            .MaxAsync(cancellationToken) ?? 0;

        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = trimmedName,
            Quantity = createIngredientRequest.Quantity,
            Unit = createIngredientRequest.Unit,
            Note = trimmedNote,
            Position = nextPosition + 1,
            CreatedAt = DateTime.UtcNow,
            IngredientGroupId = createIngredientRequest.IngredientGroupId,
            RecipeId = recipeId
        };

        _context.Ingredients.Add(ingredient);
        
        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult<CreateIngredientResponse>.Success(new CreateIngredientResponse
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            Unit = ingredient.Unit,
            Note = ingredient.Note,
            Position = ingredient.Position,
            CreatedAt = ingredient.CreatedAt,
            IngredientGroupId = ingredient.IngredientGroupId,
            RecipeId = ingredient.RecipeId
        });
    }

    public async Task<GetIngredientResponse?> GetIngredientByIdAsync(Guid ingredientId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var ingredient = await _context.Ingredients  
            .AsNoTracking()
            .Include(i => i.Recipe)
            .Where(i => i.Id == ingredientId && i.Recipe.CreatedBy == currentUserId && !i.Recipe.IsDeleted && !i.Recipe.IsTrashed)
            .Select(i => new GetIngredientResponse
            {
                Id = i.Id,
                Name = i.Name,
                Quantity = i.Quantity,
                Unit = i.Unit,
                Note = i.Note,
                Position = i.Position,
                CreatedAt = i.CreatedAt,
                IngredientGroupId = i.IngredientGroupId,
                RecipeId = i.RecipeId
            })
            .FirstOrDefaultAsync(cancellationToken);

        return ingredient;
    }

    public async Task<OperationResult<UpdateIngredientResponse>> UpdateIngredientAsync(Guid ingredientId, Guid currentUserId, UpdateIngredientRequest updateIngredientRequest, CancellationToken cancellationToken = default)
    {
        var ingredient = await _context.Ingredients
            .Include(i => i.Recipe)
            .Where(i => i.Id == ingredientId && i.Recipe.CreatedBy == currentUserId && !i.Recipe.IsDeleted && !i.Recipe.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (ingredient == null)
        {
            return OperationResult<UpdateIngredientResponse>.Failure("Ингредиент не найден", 404);
        }

        var response = UpdateItemHelper.ApplyIngredientUpdate(ingredient, "Ingredient", currentUserId, updateIngredientRequest, _logger);

        if (response.IsSuccess && response.Value.WasUpdated)
        {
            await _context.SaveChangesAsync(cancellationToken); 
        }

        return response;
    }

    public async Task<OperationResult> DeleteIngredientByIdAsync(Guid ingredientId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var ingredient = await _context.Ingredients
            .Include(i => i.Recipe)
            .Where(i => i.Id == ingredientId && i.Recipe.CreatedBy == currentUserId && !i.Recipe.IsDeleted && !i.Recipe.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (ingredient == null)
        {
            return OperationResult.Failure("Ингредиент не найден или уже удален", 404);
        }

        _context.Remove(ingredient);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Ингредиент {IngredientId} удален пользователем {UserId}",
            ingredientId, currentUserId); 


        return OperationResult.Success();
    }

    public async Task<OperationResult> ReorderIngredientsAsync(Guid recipeId, Guid currentUserId, IReadOnlyList<Guid> orderedIds, CancellationToken cancellationToken = default)
    {
        var recipeExists = await _context.Recipes
            .AnyAsync(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed, cancellationToken);
        
        if (!recipeExists) 
        {
            return OperationResult.Failure("Рецепт не найден", 404);
        }

        var ingredients = await _context.Ingredients
            .Where(i => i.RecipeId == recipeId)
            .ToListAsync(cancellationToken);

        if (ingredients.Count != orderedIds.Count) {
            return OperationResult.Failure("Неверный набор ингредиентов", 400);
        } 

        var map = ingredients.ToDictionary(x => x.Id);

        if (orderedIds.Distinct().Count() != orderedIds.Count)
        {
            return OperationResult.Failure("orderedIds содержат дубликаты", 400);
        }

        for (int i = 0; i < orderedIds.Count; i++)
        {
            if (!map.TryGetValue(orderedIds[i], out var ingredient))
            {
                return OperationResult.Failure("Переданы чужие/несуществующие ингредиенты", 400);
            } 

            ingredient.Position = i + 1;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    // Группа ингредиентов 

    public async Task<OperationResult<CreateIngredientGroupResponse>> CreateIngredientGroupAsync(Guid recipeId, CreateIngredientGroupRequest createIngredientGroupRequest, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var trimmedTitle = createIngredientGroupRequest.Title.Trim();

        if (string.IsNullOrEmpty(trimmedTitle))
        {
            return OperationResult<CreateIngredientGroupResponse>.Failure("Название группы не может быть пустым", 400);
        }

        // Проверяем, что рецепт существует и принадлежит пользователю
        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            return OperationResult<CreateIngredientGroupResponse>.Failure("Рецепт не найден", 404);
        }
        
        var ingredientGroup = new IngredientGroup
        {
            Id = Guid.NewGuid(),
            Title = trimmedTitle,
            CreatedAt = DateTime.UtcNow,
            RecipeId = recipeId
        };

        _context.IngredientGroups.Add(ingredientGroup);
        
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Создана группа ингредиентов {groupId} для рецепта {recipeId}", ingredientGroup.Id, recipe.Id);

        return OperationResult<CreateIngredientGroupResponse>.Success(new CreateIngredientGroupResponse
        {
            Id = ingredientGroup.Id,
            Title = ingredientGroup.Title,
            CreatedAt = ingredientGroup.CreatedAt,
            RecipeId = ingredientGroup.RecipeId
        });
    }

    public async Task<GetIngredientGroupResponse?> GetIngredientGroupByIdAsync(Guid ingredientGroupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var ingredientGroup = await _context.IngredientGroups
            .AsNoTracking()
            .Include(ig => ig.Recipe)
            .Where(ig => ig.Id == ingredientGroupId && ig.Recipe.CreatedBy == currentUserId && !ig.Recipe.IsDeleted)
            .Select(ig => new GetIngredientGroupResponse
            {
                Id = ig.Id,
                Title = ig.Title,
                CreatedAt = ig.CreatedAt,
                RecipeId = ig.RecipeId
            })
            .FirstOrDefaultAsync(cancellationToken);

        return ingredientGroup;
    }

    public async Task<OperationResult<UpdateIngredientGroupResponse>> UpdateIngredientGroupAsync(Guid ingredientGroupId, Guid currentUserId, UpdateIngredientGroupRequest updateIngredientGroupRequest, CancellationToken cancellationToken = default)
    {
        var ingredientGroup = await _context.IngredientGroups
            .Include(ig => ig.Recipe)
            .Where(ig => ig.Id == ingredientGroupId && ig.Recipe.CreatedBy == currentUserId && !ig.Recipe.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (ingredientGroup == null)
        {
            return OperationResult<UpdateIngredientGroupResponse>.Failure("Группа ингредиентов не найдена", 404);
        }

        if (updateIngredientGroupRequest.Title == null)
        {
            return OperationResult<UpdateIngredientGroupResponse>.Failure("Ни одного параметра не было передано", 400);
        }

        var trimmedTitle = updateIngredientGroupRequest.Title.Trim();

        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            return OperationResult<UpdateIngredientGroupResponse>.Failure("Название не может быть пустым", 400);
        }

        var wasUpdated = false;

        if (trimmedTitle != ingredientGroup.Title)
        {
            ingredientGroup.Title = trimmedTitle;
            wasUpdated = true;
            _logger.LogInformation(
                "Название группы ингредиентов {groupId} обновлено пользователем {userId}",
                ingredientGroup.Id, currentUserId
            );
        }

        if (wasUpdated)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _logger.LogInformation("Новые данные соответствуют старым, изменения не применены.");
        }

        return OperationResult<UpdateIngredientGroupResponse>.Success(new UpdateIngredientGroupResponse
        {
            Id = ingredientGroup.Id,
            Title = ingredientGroup.Title,
            WasUpdated = wasUpdated
        });
    }

    public async Task<OperationResult> DeleteIngredientGroupByIdAsync(Guid ingredientGroupId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var ingredientGroup = await _context.IngredientGroups
            .Include(ig => ig.Recipe)
            .Where(ig => ig.Id == ingredientGroupId && ig.Recipe.CreatedBy == currentUserId && !ig.Recipe.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (ingredientGroup == null)
        {
            return OperationResult.Failure("Группа ингредиентов не найдена", 404);
        }

        // Удаляем связь с группой у всех ингредиентов в этой группе
        var ingredientsInGroup = await _context.Ingredients
            .Where(i => i.IngredientGroupId == ingredientGroupId)
            .ToListAsync(cancellationToken);

        foreach (var ingredient in ingredientsInGroup)
        {
            ingredient.IngredientGroupId = null;
        }

        _context.IngredientGroups.Remove(ingredientGroup);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Группа ингредиентов {groupId} удалена пользователем {userId}, ингредиенты ({count}) очищены",
            ingredientGroupId, currentUserId, ingredientsInGroup.Count);

        return OperationResult.Success();
    }

    public async Task<GetRecipeStepResponse?> GetRecipeStepAsync(Guid recipeStepId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipeStep = await _context.RecipeSteps
            .AsNoTracking()
            .Include(rs => rs.Recipe)
            .Where(rs => rs.Id == recipeStepId && rs.Recipe.CreatedBy == currentUserId && !rs.Recipe.IsDeleted && !rs.Recipe.IsTrashed)
            .Select(rs => new GetRecipeStepResponse
            {
                Id = rs.Id,
                Description = rs.Description,
                Position = rs.Position,
                CreatedAt = rs.CreatedAt,
                RecipeId = rs.RecipeId
            })
            .FirstOrDefaultAsync(cancellationToken);

        return recipeStep;
    }

    public async Task<CreateRecipeStepResponse> CreateRecipeStepAsync(Guid recipeId, CreateRecipeStepRequest createRecipeStepRequest, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var trimmedDescription = createRecipeStepRequest.Description?.Trim();

        if (createRecipeStepRequest.Description != null && string.IsNullOrEmpty(trimmedDescription))
        {
            throw new ArgumentException("Описание не может быть пустым");
        }

        // Проверяем, что рецепт существует и принадлежит пользователю
        var recipe = await _context.Recipes
            .Where(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe == null)
        {
            throw new InvalidOperationException("Рецепт не найден");
        }

        var nextPosition = await _context.RecipeSteps
            .Where(rs => rs.RecipeId == recipeId)
            .Select(rs => (int?)rs.Position)
            .MaxAsync(cancellationToken) ?? 0;

        var recipeStep = new RecipeStep
        {
            Id = Guid.NewGuid(),
            Description = trimmedDescription,
            Position = nextPosition + 1,
            CreatedAt = DateTime.UtcNow,
            RecipeId = recipeId
        };

        _context.RecipeSteps.Add(recipeStep);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Создан шаг рецепта {stepId} для рецепта {recipeId}", recipeStep.Id, recipe.Id);

        return new CreateRecipeStepResponse
        {
            Id = recipeStep.Id,
            Description = recipeStep.Description,
            Position = recipeStep.Position,
            CreatedAt = recipeStep.CreatedAt,
            RecipeId = recipeStep.RecipeId
        };
    }

    public async Task<OperationResult<UpdateRecipeStepResponse>> UpdateRecipeStepAsync(Guid recipeStepId, Guid currentUserId, UpdateRecipeStepRequest updateRecipeStepRequest, CancellationToken cancellationToken = default)
    {
        var recipeStep = await _context.RecipeSteps
            .Include(rs => rs.Recipe)
            .Where(rs => rs.Id == recipeStepId && rs.Recipe.CreatedBy == currentUserId && !rs.Recipe.IsDeleted && !rs.Recipe.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipeStep == null)
        {
            return OperationResult<UpdateRecipeStepResponse>.Failure("Шаг рецепта не найден", 404);
        }

        var trimmedDescription = updateRecipeStepRequest.Description?.Trim();

        if (updateRecipeStepRequest.Description != null && string.IsNullOrWhiteSpace(trimmedDescription))
        {
            return OperationResult<UpdateRecipeStepResponse>.Failure("Описание не может быть пустым", 400);
        }

        var wasUpdated = false;

        if (trimmedDescription != recipeStep.Description)
        {
            recipeStep.Description = trimmedDescription;
            wasUpdated = true;
            _logger.LogInformation(
                "Описание шага рецепта {stepId} обновлено пользователем {userId}",
                recipeStep.Id, currentUserId
            );
        }

        if (wasUpdated)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _logger.LogInformation("Новые данные соответствуют старым, изменения не применены.");
        }

        return OperationResult<UpdateRecipeStepResponse>.Success(new UpdateRecipeStepResponse
        {
            Id = recipeStep.Id,
            Description = recipeStep.Description,
            WasUpdated = wasUpdated
        });
    }

    public async Task<OperationResult> DeleteRecipeStepByIdAsync(Guid recipeStepId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var recipeStep = await _context.RecipeSteps
            .Include(rs => rs.Recipe)
            .Where(rs => rs.Id == recipeStepId && rs.Recipe.CreatedBy == currentUserId && !rs.Recipe.IsDeleted && !rs.Recipe.IsTrashed)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipeStep == null)
        {
            return OperationResult.Failure("Шаг рецепта не найден или уже удален", 404);
        }

        _context.RecipeSteps.Remove(recipeStep);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Шаг рецепта {stepId} удален пользователем {userId}",
            recipeStepId, currentUserId);

        return OperationResult.Success();
    }

    public async Task<OperationResult> ReorderRecipeStepsAsync(Guid recipeId, Guid currentUserId, IReadOnlyList<Guid> orderedIds, CancellationToken cancellationToken = default)
    {
        var recipeExists = await _context.Recipes
            .AnyAsync(r => r.Id == recipeId && r.CreatedBy == currentUserId && !r.IsDeleted && !r.IsTrashed, cancellationToken);
        
        if (!recipeExists) 
        {
            return OperationResult.Failure("Рецепт не найден", 404);
        }

        var steps = await _context.RecipeSteps
            .Where(i => i.RecipeId == recipeId)
            .ToListAsync(cancellationToken);

        if (steps.Count != orderedIds.Count) {
            return OperationResult.Failure("Неверный набор шагов", 400);
        } 

        var map = steps.ToDictionary(x => x.Id);
        
        for (int i = 0; i < orderedIds.Count; i++)
        {
            if (!map.TryGetValue(orderedIds[i], out var ingredient))
            {
                return OperationResult.Failure("Переданы чужие/несуществующие шаги", 400);
            } 

            ingredient.Position = i + 1;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }
}
