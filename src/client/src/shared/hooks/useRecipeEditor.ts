import { useCallback, useEffect, useRef, useState } from "react";
import useRecipes from "./useRecipes";
import { Unit } from "../enums/unit";
import { GetRecipeResponse } from "@/features/recipes/recipe";

function useRecipeEditor(id: string) {
  const [recipe, setRecipe] = useState<GetRecipeResponse | null>();
  const [isLoading, setIsLoading] = useState(true);

  const titleSaveTimeoutRef = useRef<number | null>(null);
  const descriptionSaveTimeoutRef = useRef<number | null>(null);
  const ingredientNameSaveTimeoutsRef = useRef<Map<string, number>>(new Map());
  const ingredientQuantitySaveTimeoutsRef = useRef<Map<string, number>>(
    new Map(),
  );
  const ingredientUnitSaveTimeoutsRef = useRef<Map<string, number>>(new Map());

  const {
    getRecipeById,
    updateTitle,
    updateDescription,
    updateIngredientName,
    updateIngredientQuantity,
    updateIngredientUnit,
    createIngredient,
    deleteIngredient,
    createStep,
    deleteStep,
  } = useRecipes();

  if (!id) {
    return {
      recipe: null,
      isLoading: false,
      actions: {
        handleTitleChange: () => {},
        handleDescriptionChange: () => {},
        handleIngredientNameChange: () => {},
        handleIngredientQuantityChange: () => {},
        handleIngredientUnitChange: () => {},
        handleCreateIngredient: async () => {},
        handleDeleteIngredient: async () => {},
        handleCreateStep: async () => {},
        handleDeleteStep: async () => {},
      },
    };
  }

  useEffect(() => {
    (async () => {
      setIsLoading(true);
      const fetchedRecipe = await getRecipeById(id);
      if (fetchedRecipe) {
        setRecipe(fetchedRecipe);
        console.log("recipe: ", fetchedRecipe);
      }
      setIsLoading(false);
    })();
  }, [id, getRecipeById]);

  // ── Очистка таймеров при размонтировании компонента ──
  useEffect(() => {
    return () => {
      if (titleSaveTimeoutRef.current) {
        window.clearTimeout(titleSaveTimeoutRef.current);
      }
      if (descriptionSaveTimeoutRef.current) {
        window.clearTimeout(descriptionSaveTimeoutRef.current);
      }
      ingredientNameSaveTimeoutsRef.current.forEach((timeoutId) => {
        window.clearTimeout(timeoutId);
      });
      ingredientNameSaveTimeoutsRef.current.clear();
      ingredientQuantitySaveTimeoutsRef.current.forEach((timeoutId) => {
        window.clearTimeout(timeoutId);
      });
      ingredientQuantitySaveTimeoutsRef.current.clear();
      ingredientUnitSaveTimeoutsRef.current.forEach((timeoutId) => {
        window.clearTimeout(timeoutId);
      });
      ingredientUnitSaveTimeoutsRef.current.clear();
    };
  }, []);

  // ── Обработчик изменения заголовка рецепта ──
  const handleTitleChange = useCallback(
    (title: string) => {
      setRecipe((prev) => (prev ? { ...prev, title } : prev));

      // очистка предыдущего таймера
      if (titleSaveTimeoutRef.current) {
        window.clearTimeout(titleSaveTimeoutRef.current);
      }

      titleSaveTimeoutRef.current = window.setTimeout(async () => {
        try {
          await updateTitle({ id, title });
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000); // 1 секунда паузы после ввода
    },
    [id, updateTitle],
  );

  // ── Обработчик изменения описания рецепта ──
  const handleDescriptionChange = useCallback(
    (description: string) => {
      setRecipe((prev) => (prev ? { ...prev, description } : prev));

      // очистка предыдущего таймера
      if (descriptionSaveTimeoutRef.current) {
        window.clearTimeout(descriptionSaveTimeoutRef.current);
      }

      descriptionSaveTimeoutRef.current = window.setTimeout(async () => {
        try {
          await updateDescription({ id, description });
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000); // 1 секунда паузы после ввода
    },
    [id, updateDescription],
  );

  // ── Обработчик создания ингредиента ──
  const handleCreateIngredient = useCallback(async () => {
    try {
      if (!id) return;
      await createIngredient(id);
      const fetchedRecipe = await getRecipeById(id);
      if (fetchedRecipe) {
        setRecipe(fetchedRecipe);
      }
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, [id, createIngredient, getRecipeById]);

  // ── Обработчик удаления ингредиента ──
  const handleDeleteIngredient = useCallback(
    async (ingredientId: string) => {
      try {
        if (!ingredientId || !id) return;
        await deleteIngredient(ingredientId);
        const fetchedRecipe = await getRecipeById(id);
        if (fetchedRecipe) {
          setRecipe(fetchedRecipe);
        }
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [id, deleteIngredient, getRecipeById],
  );

  // ── Обработчик изменения названия ингредиента ──
  const handleIngredientNameChange = useCallback(
    (ingredientId: string, name: string) => {
      setRecipe((prev) => {
        if (!prev) return prev;

        const ingredients = prev.ingredients.map((ingredient) =>
          ingredient.id === ingredientId ? { ...ingredient, name } : ingredient,
        );

        return { ...prev, ingredients };
      });

      const existingTimeout =
        ingredientNameSaveTimeoutsRef.current.get(ingredientId);

      if (existingTimeout) {
        window.clearTimeout(existingTimeout);
      }

      const timeoutId = window.setTimeout(async () => {
        try {
          await updateIngredientName(ingredientId, name);
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000); // 1 секунда паузы после ввода

      ingredientNameSaveTimeoutsRef.current.set(ingredientId, timeoutId);
    },
    [updateIngredientName],
  );

  // ── Обработчик изменения количества ингредиента ──
  const handleIngredientQuantityChange = useCallback(
    (ingredientId: string, quantity: number) => {
      setRecipe((prev) => {
        if (!prev) return prev;

        const ingredients = prev.ingredients.map((ingredient) =>
          ingredient.id === ingredientId
            ? { ...ingredient, quantity }
            : ingredient,
        );

        return { ...prev, ingredients };
      });

      const existingTimeout =
        ingredientQuantitySaveTimeoutsRef.current.get(ingredientId);

      if (existingTimeout) {
        window.clearTimeout(existingTimeout);
      }

      const timeoutId = window.setTimeout(async () => {
        try {
          await updateIngredientQuantity(ingredientId, quantity);
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000);

      ingredientQuantitySaveTimeoutsRef.current.set(ingredientId, timeoutId);
    },
    [updateIngredientQuantity],
  );

  // ── Обработчик изменения единицы измерения ингредиента ──
  const handleIngredientUnitChange = useCallback(
    (ingredientId: string, unit: Unit) => {
      setRecipe((prev) => {
        if (!prev) return prev;

        const ingredients = prev.ingredients.map((ingredient) =>
          ingredient.id === ingredientId ? { ...ingredient, unit } : ingredient,
        );

        return { ...prev, ingredients };
      });

      const existingTimeout =
        ingredientUnitSaveTimeoutsRef.current.get(ingredientId);

      if (existingTimeout) {
        window.clearTimeout(existingTimeout);
      }

      const timeoutId = window.setTimeout(async () => {
        try {
          await updateIngredientUnit(ingredientId, unit);
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000);

      ingredientUnitSaveTimeoutsRef.current.set(ingredientId, timeoutId);
    },
    [updateIngredientUnit],
  );

  // ── Обработчик создания шага ──
  const handleCreateStep = useCallback(async () => {
    try {
      if (!id) return;
      await createStep(id);
      const fetchedRecipe = await getRecipeById(id);
      if (fetchedRecipe) {
        setRecipe(fetchedRecipe);
      }
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, [id, createStep, getRecipeById]);

  // ── Обработчик удаления шага ──
  const handleDeleteStep = useCallback(
    async (stepId: string) => {
      try {
        if (!stepId || !id) return;
        await deleteStep(stepId);
        const fetchedRecipe = await getRecipeById(id);
        if (fetchedRecipe) {
          setRecipe(fetchedRecipe);
        }
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [id, deleteStep, getRecipeById],
  );

  return {
    recipe,
    isLoading,
    actions: {
      handleTitleChange,
      handleDescriptionChange,
      handleIngredientNameChange,
      handleIngredientQuantityChange,
      handleIngredientUnitChange,
      handleCreateIngredient,
      handleDeleteIngredient,
      handleCreateStep,
      handleDeleteStep,
    },
  };
}

export default useRecipeEditor;
