import { GetRecipeResponse } from "@/features/recipes/recipe";
import { createContext, useContext } from "react";

interface RecipesContextTypes {
  recipesData: GetRecipeResponse[];
  fetchRecipes: () => Promise<void>;
  createRecipe: () => Promise<void>;
  getRecipeById: (id: string) => Promise<GetRecipeResponse | undefined>;
  updateTitle: ({ id, title }: { id: string; title: string }) => Promise<void>;
  updateDescription: ({
    id,
    description,
  }: {
    id: string;
    description: string;
  }) => Promise<void>;
  deleteRecipe: (id: string) => Promise<void>;
  trashRecipe: (id: string) => Promise<void>;
  getTrashed: () => Promise<GetRecipeResponse[]>;
  restoreRecipe: (id: string) => Promise<void>;
  favoriteRecipe: (id: string) => Promise<void>;
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
