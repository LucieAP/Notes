import { GetRecipeStepResponse } from "@/features/recipes/recipeStep";
import { useState } from "react";

interface Props {
  step?: GetRecipeStepResponse;
  // onStepChange?:
  onDeleteStep?: (stepId: string) => void | Promise<void>;
}

function StepRow({ step, onDeleteStep }: Props) {
  const [hovered, setHovered] = useState(false);
  const [description, setDescription] = useState(step?.description);
  if (!step) return null;

  return (
    <section
      className="flex w-full"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <textarea
        className="bg-transparent border-none outline-none text-white text-sm flex-1 resize-none overflow-hidden leading-relaxed placeholder-[#555]"
        placeholder="Введите шаг приготовления"
        rows={1}
        // value={description}
      />
      <button
        onClick={() => {
          onDeleteStep?.(step.id);
          console.log("Удален step: ", step.id);
        }}
        className="text-[#666] hover:text-white text-xs px-1 py-0.5 rounded bg-transparent border-none cursor-pointer transition-all duration-150 shrink-0 mt-px"
        aria-label="Удалить шаг"
        style={{ opacity: hovered ? 1 : 0 }}
      >
        ✕
      </button>
    </section>
  );
}

export default StepRow;
