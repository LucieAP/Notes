import api from "@/shared/api/axios";
import {
  CreateRecipeRequest,
  GetRecipeResponse,
  ToggleFavoriteResponse,
  UpdateRecipeDescriptionRequest,
  UpdateRecipeResponse,
  UpdateRecipeTitleRequest,
} from "./recipe";

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
};
