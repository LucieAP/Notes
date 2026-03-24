import { GetRecipeResponse } from "@/features/recipes/recipe";
import StepRow from "./StepRow";

interface Props {
  recipe?: GetRecipeResponse;
  onCreateStep?: () => void | Promise<void>;
  // onStepChange?:
  onDeleteStep?: (stepId: string) => void | Promise<void>;
}

function StepSection({ recipe, onCreateStep, onDeleteStep }: Props) {
  if (!recipe) return null;

  const steps = recipe.steps ?? [];

  return (
    <section className="w-full flex flex-col items-start gap-2 py-1">
      <span className="text-[#888] text-sm min-w-[24px] text-right select-none shrink-0 pt-px">
        Шаги:
      </span>

      {steps.map((step) => (
        <StepRow key={step.id} step={step} onDeleteStep={onDeleteStep} />
      ))}

      <div className="flex gap-1 mt-2">
        <button
          onClick={onCreateStep}
          className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150"
        >
          <span>+</span> шаг
        </button>

        {/* <button className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150">
          <span className="text-sm leading-none">+</span> подгруппа
        </button> */}
      </div>
    </section>
  );
}

export default StepSection;
