import { useEffect, useRef, useState } from "react";
import IngredientSection from "./IngredientSection";
import RecipeDivider from "./RecipeDivider";
import StepSection from "./StepSection";

import "@/styles/recipes.scss";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import { Unit } from "@/shared/enums/unit";

interface Props {
  recipe?: GetRecipeResponse;
  actions?: {
    handleTitleChange?: (title: string) => void;
    handleDescriptionChange?: (description: string) => void;
    handleIngredientNameChange?: (ingredientId: string, name: string) => void;
    handleIngredientQuantityChange?: (
      ingredientId: string,
      quantity: number,
    ) => void;
    handleIngredientUnitChange?: (ingredientId: string, unit: Unit) => void;
    handleCreateIngredient?: () => void | Promise<void>;
    handleDeleteIngredient?: (ingredientId: string) => void | Promise<void>;
    handleCreateStep?: () => void | Promise<void>;
    handleStepDescriptionChange?: (stepId: string, description: string) => void;
    handleDeleteStep?: (stepId: string) => void | Promise<void>;
  };
}

function RecipesEditor({ recipe, actions }: Props) {
  const titleRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (titleRef.current && recipe?.title != titleRef.current.innerText) {
      titleRef.current.innerText = recipe?.title ?? "";
    }
  }, [recipe?.title]);

  return (
    <div
      className="flex flex-col w-full items-center h-full min-w-0"
      style={{ fontFamily: '"Inter", sans-serif' }}
    >
      <div className="flex flex-col my-10 w-full max-w-3xl min-w-0 px-4 sm:px-6">
        {/* ── Заголовок ── */}
        <div
          ref={titleRef}
          className="text-[2.2rem] font-bold text-(--text-primary) outline-none mb-3 
                    min-h-10 cursor-text leading-[1.2] wrap-break-word whitespace-pre-wrap break-words hyphens-auto
                    [&:empty::before]:content-[attr(data-placeholder)] 
                    [&:empty::before]:text-(--text-placeholder) 
                    [&:empty::before]:pointer-events-none 
                    bg-transparent pt-12 p w-full"
          style={{ overflowWrap: "anywhere", wordBreak: "break-word" }}
          contentEditable
          suppressContentEditableWarning
          spellCheck={false}
          data-placeholder="Название рецепта"
          onInput={(e) =>
            actions?.handleTitleChange?.(e.currentTarget.textContent ?? "")
          }
        ></div>

        <RecipeDivider />

        {/* ── Описание ── */}
        <textarea
          className="w-full bg-transparent border-none outline-none text-(--text-secondary)
         text-[0.95rem] resize-none leading-[1.6] p-0 font-inherit placeholder:text-(--text-placeholder) break-words hyphens-auto"
          placeholder="Описание"
          rows={3}
          value={recipe?.description ?? ""}
          spellCheck={false}
          style={{ overflowWrap: "anywhere", wordBreak: "break-word" }}
          onChange={(e) =>
            actions?.handleDescriptionChange?.(e.target.value ?? "")
          }
        />
        <RecipeDivider />

        {/* ── Ингредиенты ── */}

        <IngredientSection
          recipe={recipe}
          onIngredientNameChange={actions?.handleIngredientNameChange}
          onIngredientQuantityChange={actions?.handleIngredientQuantityChange}
          onIngredientUnitChange={actions?.handleIngredientUnitChange}
          onCreateIngredient={actions?.handleCreateIngredient}
          onDeleteIngredient={actions?.handleDeleteIngredient}
        />

        <RecipeDivider />

        {/* ── Методы ── */}

        <StepSection
          recipe={recipe}
          onCreateStep={actions?.handleCreateStep}
          onChangeStep={actions?.handleStepDescriptionChange}
          onDeleteStep={actions?.handleDeleteStep}
        />
      </div>
    </div>
  );
}

export default RecipesEditor;
