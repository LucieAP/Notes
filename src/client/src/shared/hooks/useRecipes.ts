import { GetRecipeResponse } from "@/features/recipes/recipe";
import { createContext, useContext } from "react";
import { Unit } from "../enums/unit";

interface RecipesContextTypes {
  recipesData: GetRecipeResponse[];
  isLoading: boolean;
  fetchRecipes: () => Promise<void>;
  createRecipe: () => Promise<void>;
  getRecipeById: (recipeId: string) => Promise<GetRecipeResponse | undefined>;
  updateTitle: ({ id, title }: { id: string; title: string }) => Promise<void>;
  updateDescription: ({
    recipeId,
    description,
  }: {
    recipeId: string;
    description: string;
  }) => Promise<void>;
  deleteRecipe: (recipeId: string) => Promise<void>;
  trashRecipe: (recipeId: string) => Promise<void>;
  getRecipesTrashed: () => Promise<GetRecipeResponse[]>;
  restoreRecipe: (recipeId: string) => Promise<void>;
  toggleFavorite: (recipeId: string) => Promise<void>;
  createIngredient: (recipeId: string) => Promise<void>;
  updateIngredientName: (ingredientId: string, name: string) => Promise<void>;
  updateIngredientQuantity: (
    ingredientId: string,
    quantity: number,
  ) => Promise<void>;
  updateIngredientUnit: (ingredientId: string, unit: Unit) => Promise<void>;
  deleteIngredient: (ingredientId: string) => Promise<void>;
  reorderIngredients: ({
    recipeId,
    orderedIngredientIds,
  }: {
    recipeId: string;
    orderedIngredientIds: string[];
  }) => Promise<void>;
  createStep: (recipeId: string) => Promise<void>;
  updateStepDescription: (
    stepId: string,
    description: string | null,
  ) => Promise<void>;
  deleteStep: (stepId: string) => Promise<void>;
  reorderSteps: ({
    recipeId,
    orderedStepIds,
  }: {
    recipeId: string;
    orderedStepIds: string[];
  }) => Promise<void>;
}

export const RecipesContext = createContext<RecipesContextTypes | null>(null);

function useRecipes() {
  const recipesContext = useContext(RecipesContext);

  if (!recipesContext) {
    throw new Error("useRecipes must be used within RecipeProvider");
  }

  return recipesContext;
}

export default useRecipes;
