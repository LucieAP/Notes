/** DTO для получения шага рецепта (GetRecipeStepResponse на сервере) */
export interface GetRecipeStepResponse {
  id: string;
  description: string | null;
  createdAt: string;
  recipeId: string;
}

/** DTO для создания шага рецепта (CreateRecipeStepRequest на сервере) */
export interface CreateRecipeStepRequest {
  description: string | null;
}

export interface CreateRecipeStepResponse {
  id: string;
  description: string | null;
  createdAt: string;
  recipeId: string;
}

/** DTO для обновления шага рецепта (UpdateRecipeStepResponse на сервере) */
export interface UpdateRecipeStepRequest {
  description: string | null;
}

export interface UpdateRecipeStepResponse {
  id: string;
  description: string | null;
  wasUpdated: boolean;
}
