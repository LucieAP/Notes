import { useEffect, useRef, useState } from "react";
import IngredientSection from "./IngredientSection";
import RecipeDivider from "./RecipeDivider";
import StepSection from "./StepSection";

import "@/styles/recipes.scss";

interface Props {
  recipeTitle?: string;
  recipeDescription?: string;
  onTitleChange?: (title: string) => void;
  onDescriptionChange?: (description: string) => void;
}

function RecipesEditor({
  recipeTitle,
  recipeDescription,
  onTitleChange,
  onDescriptionChange,
}: Props) {
  const titleRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (titleRef.current && recipeTitle != titleRef.current.innerText) {
      titleRef.current.innerText = recipeTitle ?? "";
    }
  }, [recipeTitle]);

  return (
    <div
      className="flex flex-col w-full items-center h-full"
      style={{ fontFamily: '"Inter", sans-serif' }}
    >
      <div className="flex flex-col my-10 w-full max-w-3xl ">
        {/* ── Заголовок ── */}
        <div
          ref={titleRef}
          className="text-[2.2rem] font-bold text-(--text-primary) outline-none mb-3 
                    min-h-10 cursor-text leading-[1.2] wrap-break-word whitespace-pre-wrap 
                    [&:empty::before]:content-[attr(data-placeholder)] 
                    [&:empty::before]:text-(--text-placeholder) 
                    [&:empty::before]:pointer-events-none 
                    bg-transparent pt-12 p w-full"
          contentEditable
          suppressContentEditableWarning
          data-placeholder="Название рецепта"
          onInput={(e) => onTitleChange?.(e.currentTarget.textContent ?? "")}
        ></div>

        <RecipeDivider />

        {/* ── Описание ── */}
        <textarea
          className="w-full bg-transparent border-none outline-none text-(--text-secondary)
         text-[0.95rem] resize-none leading-[1.6] p-0 font-inherit placeholder:text-(--text-placeholder)"
          placeholder="Описание"
          rows={3}
          value={recipeDescription ?? ""}
          onChange={(e) => onDescriptionChange?.(e.target.value ?? "")}
        />
        <RecipeDivider />

        {/* ── Ингредиенты ── */}

        <IngredientSection />

        <RecipeDivider />

        {/* ── Методы ── */}

        <StepSection />
      </div>
    </div>
  );
}

export default RecipesEditor;
