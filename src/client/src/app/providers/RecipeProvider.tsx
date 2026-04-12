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

      setRecipesData((prev) => [...prev, createdRecipe]);
      console.log(`createdRecipe: ${createdRecipe}`);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const getRecipeById = useCallback(
    async (id: string) => {
      try {
        if (!id) return;

        const cachedRecipe = recipesData.find((recipe) => recipe.id === id);
        if (cachedRecipe) return cachedRecipe;

        const recipe = await recipesApi.getRecipeById(id);
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
          prev.map((recipe) =>
            recipe.id === id ? { ...recipe, title } : recipe,
          ),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const updateDescription = useCallback(
    async ({ id, description }: { id: string; description: string }) => {
      try {
        if (!id) return;
        await recipesApi.updateDescription({ id, description });
        setRecipesData((prev) =>
          prev.map((recipe) =>
            recipe.id === id ? { ...recipe, description } : recipe,
          ),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const deleteRecipe = useCallback(async (id: string) => {
    try {
      if (!id) return;
      await recipesApi.delete(id);

      setRecipesData((prev) => prev.filter((recipe) => recipe.id !== id));
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const trashRecipe = useCallback(
    async (id: string) => {
      try {
        if (!id) return;
        await recipesApi.trash(id);
        fetchRecipes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchRecipes],
  );

  const restoreRecipe = useCallback(
    async (id: string) => {
      try {
        if (!id) return;
        await recipesApi.restore(id);
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

  const toggleFavorite = useCallback(async (id: string) => {
    try {
      if (!id) return;
      const data = await recipesApi.toggleFavorite(id);
      setRecipesData((prev) =>
        prev.map((recipe) =>
          recipe.id === id
            ? { ...recipe, isFavorite: data.isFavorite }
            : recipe,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  // Ингредиенты

  const createIngredient = useCallback(async (id: string) => {
    try {
      if (!id) return;

      const createdIngredient = await recipesApi.createIngredient({
        id,
        data: {
          unit: Unit.Default,
        },
      });

      if (!createdIngredient) return;
      console.log(`createdIngredient: ${createdIngredient}`);

      setRecipesData((prev) =>
        prev.map((recipe) =>
          recipe.id === id
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

  const updateIngredientName = useCallback(async (id: string, name: string) => {
    try {
      if (!id) return;

      await recipesApi.updateIngredient({ id, data: { name: name } });

      setRecipesData((prev) =>
        prev.map((recipe) => ({
          ...recipe,
          ingredients: recipe.ingredients.map((ingredient) =>
            ingredient.id === id ? { ...ingredient, name: name } : ingredient,
          ),
        })),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const updateIngredientQuantity = useCallback(
    async (id: string, quantity: number) => {
      try {
        if (!id) return;

        await recipesApi.updateIngredient({ id, data: { quantity: quantity } });

        setRecipesData((prev) =>
          prev.map((recipe) => ({
            ...recipe,
            ingredients: recipe.ingredients.map((ingredient) =>
              ingredient.id === id
                ? { ...ingredient, quantity: quantity }
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

  const updateIngredientUnit = useCallback(async (id: string, unit: Unit) => {
    try {
      await recipesApi.updateIngredient({ id, data: { unit: unit } });

      setRecipesData((prev) =>
        prev.map((recipe) => ({
          ...recipe,
          ingredients: recipe.ingredients.map((ingredient) =>
            ingredient.id === id ? { ...ingredient, unit: unit } : ingredient,
          ),
        })),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const deleteIngredient = useCallback(async (id: string) => {
    try {
      if (!id) return;

      await recipesApi.deleteIngredient(id);

      setRecipesData((prev) =>
        prev.map((recipe) => ({
          ...recipe,
          ingredients: recipe.ingredients.filter(
            (ingredient) => ingredient.id !== id,
          ),
        })),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const reorderIngredients = useCallback(
    async ({
      id,
      orderedIngredientIds,
    }: {
      id: string;
      orderedIngredientIds: string[];
    }) => {
      if (!id || orderedIngredientIds.length === 0) return;

      let previousRecipeIngredients: GetRecipeResponse["ingredients"] | null =
        null;

      setRecipesData((prev) =>
        prev.map((recipe) => {
          if (recipe.id !== id) {
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
          id,
          orderedIds: orderedIngredientIds,
        });
      } catch (err) {
        // откат
        if (previousRecipeIngredients) {
          setRecipesData((prev) =>
            prev.map((recipe) =>
              recipe.id === id
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

  const createStep = useCallback(async (id: string) => {
    try {
      if (!id) return;

      const createdStep = await recipesApi.createStep({
        id,
        data: {
          description: null,
        },
      });

      console.log("createdStep: ", createdStep);

      setRecipesData((prev) =>
        prev.map((recipe) =>
          recipe.id === id
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
    async (id: string, description: string | null) => {
      try {
        if (!id) return;

        await recipesApi.updateStep({ id, data: { description } });

        setRecipesData((prev) =>
          prev.map((recipe) => ({
            ...recipe,
            steps: recipe.steps.map((step) =>
              step.id === id ? { ...step, description } : step,
            ),
          })),
        );
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [],
  );

  const deleteStep = useCallback(async (id: string) => {
    try {
      if (!id) return;

      await recipesApi.deleteStep(id);

      setRecipesData((prev) =>
        prev.map((recipe) => ({
          ...recipe,
          steps: recipe.steps.filter((step) => step.id !== id),
        })),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const reorderSteps = useCallback(
    async ({
      id,
      orderedStepIds,
    }: {
      id: string;
      orderedStepIds: string[];
    }) => {
      if (!id || orderedStepIds.length === 0) return;

      let previousRecipeSteps: GetRecipeResponse["steps"] | null = null;

      setRecipesData((prev) =>
        prev.map((recipe) => {
          if (recipe.id != id) {
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
        await recipesApi.reorderSteps({ id, orderedIds: orderedStepIds });
      } catch (err) {
        if (previousRecipeSteps) {
          setRecipesData((prev) =>
            prev.map((recipe) =>
              recipe.id === id
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
