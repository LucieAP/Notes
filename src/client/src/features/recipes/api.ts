import api from "@/shared/api/axios";
import {
  CreateRecipeRequest,
  GetRecipeResponse,
  ToggleFavoriteResponse,
  UpdateRecipeDescriptionRequest,
  UpdateRecipeResponse,
  UpdateRecipeTitleRequest,
} from "./recipe";
import {
  CreateIngredientRequest,
  CreateIngredientResponse,
  UpdateIngredientRequest,
} from "./ingredient";
import {
  CreateRecipeStepRequest,
  CreateRecipeStepResponse,
} from "./recipeStep";

export const recipesApi = {
  getAll(): Promise<GetRecipeResponse[]> {
    return api.get("/recipes");
  },

  getRecipeById(id: string): Promise<GetRecipeResponse> {
    return api.get(`/recipes/${id}`);
  },

  create(data: CreateRecipeRequest): Promise<GetRecipeResponse> {
    return api.post("/recipes/create", data);
  },

  toggleFavorite(id: string): Promise<ToggleFavoriteResponse> {
    return api.patch(`recipes/${id}/favorite`);
  },

  updateTitle({
    id,
    title,
  }: UpdateRecipeTitleRequest): Promise<UpdateRecipeResponse> {
    return api.patch(`recipes/${id}`, { title });
  },

  updateDescription({
    id,
    description,
  }: UpdateRecipeDescriptionRequest): Promise<UpdateRecipeResponse> {
    return api.patch(`recipes/${id}`, { description });
  },

  delete(id: string) {
    return api.delete(`recipes/delete/${id}`);
  },

  trash(id: string) {
    return api.patch(`recipes/${id}/trash`);
  },

  restore(id: string) {
    return api.patch(`recipes/${id}/restore`);
  },

  getTrashed(): Promise<GetRecipeResponse[]> {
    return api.get("/recipes/trashed");
  },

  createIngredient({
    id,
    data,
  }: {
    id: string;
    data: CreateIngredientRequest;
  }): Promise<CreateIngredientResponse> {
    return api.post(`recipes/${id}/ingredient/create`, data);
  },

  updateIngredient({
    id,
    data,
  }: {
    id: string;
    data: UpdateIngredientRequest;
  }): Promise<void> {
    return api.patch(`recipes/ingredient/${id}`, data);
  },

  deleteIngredient(id: string): Promise<void> {
    return api.delete(`recipes/ingredient/delete/${id}`);
  },

  createStep({
    id,
    data,
  }: {
    id: string;
    data: CreateRecipeStepRequest;
  }): Promise<CreateRecipeStepResponse> {
    return api.post(`/recipes/${id}/step/create`, data);
  },

  deleteStep(id: string): Promise<void> {
    return api.delete(`recipes/step/delete/${id}`);
  },
};
