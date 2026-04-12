import { Unit } from "@/shared/enums/unit";

/** DTO для получения ингредиента (GetIngredientResponse на сервере) */
export interface GetIngredientResponse {
  id: string;
  name: string | null;
  quantity: number | null;
  unit: Unit | null;
  note: string | null;
  position: number;
  createdAt: string;
  ingredientGroupId: string | null;
  recipeId: string;
}

/** DTO для создания ингредиента (CreateIngredientRequest на сервере) */
export interface CreateIngredientRequest {
  name?: string | null;
  quantity?: number | null;
  unit?: Unit | null;
  note?: string | null;
  ingredientGroupId?: string | null;
}

export interface CreateIngredientResponse {
  id: string;
  name: string | null;
  quantity: number | null;
  unit: Unit | null;
  note: string | null;
  position: number;
  createdAt: string;
  ingredientGroupId: string | null;
  recipeId: string;
}

/** DTO для обновления ингредиента (UpdateIngredientRequest на сервере) */
export interface UpdateIngredientRequest {
  name?: string | null;
  quantity?: number | null;
  unit?: Unit | null;
  note?: string | null;
}

export interface UpdateIngredientResponse {
  id: string;
  name: string | null;
  quantity: number | null;
  unit: Unit | null;
  note: string | null;
  wasUpdated: boolean;
}
