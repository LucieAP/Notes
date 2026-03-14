import { User } from "@/shared/types/user";
import { RecipeGroup } from "./recipeGroup";
import { GetIngredientResponse } from "./ingredient";
import { GetRecipeStepResponse } from "./recipeStep";

export interface GetRecipeResponse {
  id: string;
  title: string;
  description: string | null;
  isFavorite: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  isDeleted: boolean;
  deletedAt: string | null;
  recipeGroup: RecipeGroup | null;
  createdBy: User;
  ingredients: GetIngredientResponse[];
  steps: GetRecipeStepResponse[];
}

export interface CreateRecipeRequest {
  title: string;
  description?: string | null;
  isFavorite: boolean;
  recipeGroupId?: string | null;
}

export interface CreateRecipeResponse {
  id: string;
  title: string;
  description: string | null;
  isFavorite: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  isDeleted: boolean;
  deletedAt: string | null;
  recipeGroupId: string | null;
  createdBy: string;
}

export interface ToggleFavoriteResponse {
  id: string;
  isFavorite: boolean;
  lastModifiedAt: string;
}

export interface UpdateRecipeTitleRequest {
  id: string;
  title: string;
}

export interface UpdateRecipeDescriptionRequest {
  id: string;
  description: string;
}

export interface UpdateRecipeResponse {
  id: string;
  title: string | null;
  description: string | null;
  lastModifiedAt: string;
  wasUpdated: boolean;
}
