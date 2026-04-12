import { GetRecipeResponse } from "@/features/recipes/recipe";
import StepRow from "./StepRow";
import { DragDropProvider, DragEndEvent } from "@dnd-kit/react";
import { isSortableOperation } from "@dnd-kit/react/sortable";

interface Props {
  recipe?: GetRecipeResponse;
  onCreateStep?: () => void | Promise<void>;
  onChangeStep?: (stepId: string, description: string) => void;
  onDeleteStep?: (stepId: string) => void | Promise<void>;
  onReorderSteps?: (orderedStepIds: string[]) => void | Promise<void>;
}

function StepSection({
  recipe,
  onCreateStep,
  onChangeStep,
  onDeleteStep,
  onReorderSteps,
}: Props) {
  if (!recipe) return null;

  const handleDragEnd: DragEndEvent = async (event) => {
    const { operation, canceled } = event;
    if (canceled || !isSortableOperation(operation)) return;

    const from = operation.source?.initialIndex;
    const to = operation.source?.index;

    if (from == null || to == null || from === to) return;

    const next = [...recipe.steps];
    const [moved] = next.splice(from, 1);
    next.splice(to, 0, moved);

    const orderedStepIds = next.map((step) => step.id);

    console.log("orderedStepIds", orderedStepIds);
    await onReorderSteps?.(orderedStepIds);
  };

  return (
    <section className="w-full">
      <span className="text-[#888] text-sm min-w-[24px] text-right select-none shrink-0 pt-px">
        Шаги:
      </span>

      <DragDropProvider onDragEnd={handleDragEnd}>
        {recipe.steps.map((step, index) => (
          <StepRow
            key={step.id}
            index={index}
            step={step}
            onDeleteStep={onDeleteStep}
            onChangeStep={onChangeStep}
          />
        ))}
      </DragDropProvider>

      <div className="flex gap-1 mt-2">
        <button
          onClick={onCreateStep}
          className="inline-flex items-center gap-1.5 text-[#c5c5c5] hover:text-white bg-[#2a2a2a]/70 hover:bg-[#353535] border border-[#3a3a3a] hover:border-[#4a4a4a] text-xs px-2.5 py-1.5 rounded-md cursor-pointer transition-all duration-150 active:scale-[0.98] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#444]"
        >
          <span className="inline-flex items-center justify-center w-4 h-4 rounded-full bg-[#2b2b2b] text-[11px] leading-none">
            +
          </span>
          шаг
        </button>

        {/* <button className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150">
          <span className="text-sm leading-none">+</span> подгруппа
        </button> */}
      </div>
    </section>
  );
}

export default StepSection;
