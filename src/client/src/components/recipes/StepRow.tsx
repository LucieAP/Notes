import { GetRecipeStepResponse } from "@/features/recipes/recipeStep";
import { useSortable } from "@dnd-kit/react/sortable";
import { useEffect, useRef, useState } from "react";
import DragAndDropButton from "../common/buttons/DragAndDropButton";

interface Props {
  index: number;
  step?: GetRecipeStepResponse;
  onChangeStep?: (stepId: string, description: string) => void;
  onDeleteStep?: (stepId: string) => void | Promise<void>;
}

function StepRow({ index, step, onChangeStep, onDeleteStep }: Props) {
  const [hovered, setHovered] = useState(false);
  const [description, setDescription] = useState<string>(
    step?.description ?? "",
  );
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  if (!step) return null;
  const displayIndex = index + 1;
  const handleRef = useRef<HTMLButtonElement | null>(null);
  const sortable = useSortable({
    id: step.id,
    index: index,
    handle: handleRef,
  });

  const resizeTextarea = () => {
    const el = textareaRef.current;
    if (!el) return;
    el.style.height = "auto";
    el.style.height = `${el.scrollHeight}px`;
  };

  useEffect(() => {
    setDescription(step.description ?? "");
  }, [step.description]);

  useEffect(() => {
    resizeTextarea();
  }, [description]);

  return (
    <section
      ref={sortable.ref}
      className="flex items-center w-full gap-2 py-1 min-h-[36px]"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <DragAndDropButton ref={handleRef} isDragging={sortable.isDragging} />

      <div className="mr-1 text-md min-w-[24px] text-right select-none shrink-0">
        <span className="text-[#888]">{displayIndex}.</span>
      </div>

      <textarea
        ref={textareaRef}
        className="bg-transparent border-none outline-none text-[#888] text-sm flex-1 resize-none placeholder-[#555] leading-6"
        placeholder="Введите шаг приготовления"
        rows={1}
        value={description}
        spellCheck={false}
        onInput={resizeTextarea}
        onPaste={() => requestAnimationFrame(resizeTextarea)}
        onChange={(e) => {
          const nextValue = e.target.value;
          setDescription(nextValue);
          onChangeStep?.(step.id, nextValue);
        }}
        style={{ overflow: "hidden" }}
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
