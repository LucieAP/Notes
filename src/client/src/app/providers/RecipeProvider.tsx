import { recipesApi } from "@/features/recipes/api";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import { Unit } from "@/shared/enums/unit";
import { RecipesContext } from "@/shared/hooks/useRecipes";
import { useCallback, useEffect, useMemo, useState } from "react";

function RecipeProvider({ children }: { children: React.ReactNode }) {
  const [recipesData, setRecipesData] = useState<GetRecipeResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchRecipes = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await recipesApi.getAll();
      setRecipesData(data ?? []);
      console.log("fetched recipes:", data);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchRecipes();
  }, [fetchRecipes]);

  const createRecipe = useCallback(async () => {
    try {
      const createdRecipe = await recipesApi.create({
        title: "New Recipe",
        description: null,
        isFavorite: false,
      });

      const fullRecipe = await recipesApi.getRecipeById(createdRecipe.id);
      setRecipesData((prev) => [...prev, fullRecipe]);
      console.log("createdRecipe: ", fullRecipe);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const getRecipeById = useCallback(
    async (recipeId: string) => {
      try {
        if (!recipeId) return;

        const cachedRecipe = recipesData.find(
          (recipe) => recipe.id === recipeId,
        );
        if (cachedRecipe) return cachedRecipe;

        const recipe = await recipesApi.getRecipeById(recipeId);
        setRecipesData((prev) => {
          const existingIndex = prev.findIndex((item) => item.id === recipe.id);
          if (existingIndex >= 0) {
            const next = [...prev];
            next[existingIndex] = recipe;
            return next;
          }
          return [...prev, recipe];
        });
        return recipe;
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [recipesData],
  );

  const updateTitle = useCallback(
    async ({ id, title }: { id: string; title: string }) => {
      try {
        if (!id) return;
        await recipesApi.updateTitle({ id, title });
        setRecipesData((prev) =>
          prev.map((recipe) => (recipe.id === id ? { ...recipe, title } : recipe)),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const updateDescription = useCallback(
    async ({
      recipeId,
      description,
    }: {
      recipeId: string;
      description: string;
    }) => {
      try {
        if (!recipeId) return;
        await recipesApi.updateDescription({ id: recipeId, description });
        setRecipesData((prev) =>
          prev.map((recipe) =>
            recipe.id === recipeId ? { ...recipe, description } : recipe,
          ),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const deleteRecipe = useCallback(async (recipeId: string) => {
    try {
      if (!recipeId) return;
      await recipesApi.delete(recipeId);

      setRecipesData((prev) =>
        prev.filter((recipe) => recipe.id !== recipeId),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const trashRecipe = useCallback(
    async (recipeId: string) => {
      try {
        if (!recipeId) return;
        await recipesApi.trash(recipeId);
        fetchRecipes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchRecipes],
  );

  const restoreRecipe = useCallback(
    async (recipeId: string) => {
      try {
        if (!recipeId) return;
        await recipesApi.restore(recipeId);
        fetchRecipes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchRecipes],
  );

  const getRecipesTrashed = useCallback(async () => {
    try {
      const data = await recipesApi.getTrashed();
      return data ?? [];
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
      throw err;
    }
  }, []);

  const toggleFavorite = useCallback(async (recipeId: string) => {
    try {
      if (!recipeId) return;
      const data = await recipesApi.toggleFavorite(recipeId);
      setRecipesData((prev) =>
        prev.map((recipe) =>
          recipe.id === recipeId
            ? { ...recipe, isFavorite: data.isFavorite }
            : recipe,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  // Ингредиенты

  const createIngredient = useCallback(async (recipeId: string) => {
    try {
      if (!recipeId) return;

      const createdIngredient = await recipesApi.createIngredient({
        id: recipeId,
        data: {
          unit: Unit.Default,
        },
      });

      if (!createdIngredient) return;
      console.log(`createdIngredient: ${createdIngredient}`);

      setRecipesData((prev) =>
        prev.map((recipe) =>
          recipe.id === recipeId
            ? {
                ...recipe,
                ingredients: [...(recipe.ingredients ?? []), createdIngredient],
              }
            : recipe,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const updateIngredientName = useCallback(
    async (ingredientId: string, name: string) => {
      try {
        if (!ingredientId) return;

        await recipesApi.updateIngredient({ id: ingredientId, data: { name } });

        setRecipesData((prev) =>
          prev.map((recipe) => ({
            ...recipe,
            ingredients: recipe.ingredients.map((ingredient) =>
              ingredient.id === ingredientId
                ? { ...ingredient, name }
                : ingredient,
            ),
          })),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const updateIngredientQuantity = useCallback(
    async (ingredientId: string, quantity: number) => {
      try {
        if (!ingredientId) return;

        await recipesApi.updateIngredient({
          id: ingredientId,
          data: { quantity },
        });

        setRecipesData((prev) =>
          prev.map((recipe) => ({
            ...recipe,
            ingredients: recipe.ingredients.map((ingredient) =>
              ingredient.id === ingredientId
                ? { ...ingredient, quantity }
                : ingredient,
            ),
          })),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const updateIngredientUnit = useCallback(
    async (ingredientId: string, unit: Unit) => {
      try {
        await recipesApi.updateIngredient({ id: ingredientId, data: { unit } });

        setRecipesData((prev) =>
          prev.map((recipe) => ({
            ...recipe,
            ingredients: recipe.ingredients.map((ingredient) =>
              ingredient.id === ingredientId
                ? { ...ingredient, unit }
                : ingredient,
            ),
          })),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const deleteIngredient = useCallback(async (ingredientId: string) => {
    try {
      if (!ingredientId) return;

      await recipesApi.deleteIngredient(ingredientId);

      setRecipesData((prev) =>
        prev.map((recipe) => ({
          ...recipe,
          ingredients: recipe.ingredients.filter(
            (ingredient) => ingredient.id !== ingredientId,
          ),
        })),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const reorderIngredients = useCallback(
    async ({
      recipeId,
      orderedIngredientIds,
    }: {
      recipeId: string;
      orderedIngredientIds: string[];
    }) => {
      if (!recipeId || orderedIngredientIds.length === 0) return;

      let previousRecipeIngredients: GetRecipeResponse["ingredients"] | null =
        null;

      setRecipesData((prev) =>
        prev.map((recipe) => {
          if (recipe.id !== recipeId) {
            return recipe;
          }

          const byId = new Map(
            recipe.ingredients.map((ingredient) => [ingredient.id, ingredient]),
          );

          if (orderedIngredientIds.length !== recipe.ingredients.length) {
            throw new Error("Invalid ingredients length");
          }

          const next = orderedIngredientIds.map((ingredientId) => {
            const ingredient = byId.get(ingredientId);
            if (!ingredient) {
              throw new Error(`Ingredient with id ${ingredientId} not found`);
            }
            return ingredient;
          });

          previousRecipeIngredients = recipe.ingredients; // снимок до перестановки

          return { ...recipe, ingredients: next };
        }),
      );

      try {
        await recipesApi.reorderIngredients({
          id: recipeId,
          orderedIds: orderedIngredientIds,
        });
      } catch (err) {
        // откат
        if (previousRecipeIngredients) {
          setRecipesData((prev) =>
            prev.map((recipe) =>
              recipe.id === recipeId
                ? { ...recipe, ingredients: previousRecipeIngredients! }
                : recipe,
            ),
          );
        }
        console.log(err instanceof Error ? err.message : err);
        throw err;
      }
    },
    [],
  );

  //  Шаги

  const createStep = useCallback(async (recipeId: string) => {
    try {
      if (!recipeId) return;

      const createdStep = await recipesApi.createStep({
        id: recipeId,
        data: {
          description: null,
        },
      });

      console.log("createdStep: ", createdStep);

      setRecipesData((prev) =>
        prev.map((recipe) =>
          recipe.id === recipeId
            ? {
                ...recipe,
                steps: [...(recipe.steps ?? []), createdStep],
              }
            : recipe,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const updateStepDescription = useCallback(
    async (stepId: string, description: string | null) => {
      try {
        if (!stepId) return;

        await recipesApi.updateStep({ id: stepId, data: { description } });

        setRecipesData((prev) =>
          prev.map((recipe) => ({
            ...recipe,
            steps: recipe.steps.map((step) =>
              step.id === stepId ? { ...step, description } : step,
            ),
          })),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const deleteStep = useCallback(async (stepId: string) => {
    try {
      if (!stepId) return;

      await recipesApi.deleteStep(stepId);

      setRecipesData((prev) =>
        prev.map((recipe) => ({
          ...recipe,
          steps: recipe.steps.filter((step) => step.id !== stepId),
        })),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const reorderSteps = useCallback(
    async ({
      recipeId,
      orderedStepIds,
    }: {
      recipeId: string;
      orderedStepIds: string[];
    }) => {
      if (!recipeId || orderedStepIds.length === 0) return;

      let previousRecipeSteps: GetRecipeResponse["steps"] | null = null;

      setRecipesData((prev) =>
        prev.map((recipe) => {
          if (recipe.id !== recipeId) {
            return recipe;
          }

          if (orderedStepIds.length !== recipe.steps.length) {
            throw new Error("Invalid steps length");
          }

          const byId = new Map(recipe.steps.map((step) => [step.id, step]));

          const next = orderedStepIds.map((stepId) => {
            const step = byId.get(stepId);
            if (!step) throw new Error(`Step with id ${stepId} not found`);
            return step;
          });

          previousRecipeSteps = recipe.steps;

          return { ...recipe, steps: next };
        }),
      );

      try {
        await recipesApi.reorderSteps({
          id: recipeId,
          orderedIds: orderedStepIds,
        });
      } catch (err) {
        if (previousRecipeSteps) {
          setRecipesData((prev) =>
            prev.map((recipe) =>
              recipe.id === recipeId
                ? { ...recipe, steps: previousRecipeSteps! }
                : recipe,
            ),
          );
        }
        console.log(err instanceof Error ? err.message : err);
        throw err;
      }
    },
    [],
  );

  const value = useMemo(() => {
    return {
      recipesData,
      isLoading,
      fetchRecipes,
      createRecipe,
      getRecipeById,
      updateTitle,
      updateDescription,
      deleteRecipe,
      trashRecipe,
      getRecipesTrashed,
      restoreRecipe,
      toggleFavorite,
      createIngredient,
      updateIngredientName,
      updateIngredientQuantity,
      updateIngredientUnit,
      deleteIngredient,
      reorderIngredients,
      createStep,
      updateStepDescription,
      deleteStep,
      reorderSteps,
    };
  }, [
    recipesData,
    isLoading,
    fetchRecipes,
    createRecipe,
    getRecipeById,
    updateTitle,
    updateDescription,
    deleteRecipe,
    trashRecipe,
    getRecipesTrashed,
    restoreRecipe,
    toggleFavorite,
    createIngredient,
    updateIngredientName,
    updateIngredientQuantity,
    updateIngredientUnit,
    deleteIngredient,
    reorderIngredients,
    createStep,
    updateStepDescription,
    deleteStep,
    reorderSteps,
  ]);

  return <RecipesContext value={value}>{children}</RecipesContext>;
}

export default RecipeProvider;
