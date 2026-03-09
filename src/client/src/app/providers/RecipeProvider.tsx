import { recipesApi } from "@/features/recipes/api";
import { GetRecipeResponse } from "@/features/recipes/recipe";
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
        fetchRecipes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchRecipes],
  );

  const updateDescription = useCallback(
    async ({ id, description }: { id: string; description: string }) => {
      try {
        if (!id) return;
        await recipesApi.updateDescription({ id, description });
        fetchRecipes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchRecipes],
  );

  const deleteRecipe = useCallback(
    async (id: string) => {
      try {
        if (!id) return;
        await recipesApi.delete(id);
        fetchRecipes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchRecipes],
  );

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

  const getTrashed = useCallback(async () => {
    try {
      const data = await recipesApi.getTrashed();
      return data ?? [];
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
      throw err;
    }
  }, []);

  const favoriteRecipe = useCallback(async (id: string) => {
    try {
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
      getTrashed,
      restoreRecipe,
      favoriteRecipe,
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
    getTrashed,
    restoreRecipe,
    favoriteRecipe,
  ]);

  return <RecipesContext value={value}>{children}</RecipesContext>;
}

export default RecipeProvider;
