import { GetRecipeStepResponse } from "@/features/recipes/recipeStep";
import { useEffect, useRef, useState } from "react";

interface Props {
  index?: number;
  step?: GetRecipeStepResponse;
  onChangeStep?: (stepId: string, description: string) => void;
  onDeleteStep?: (stepId: string) => void | Promise<void>;
}

function StepRow({ index = 1, step, onChangeStep, onDeleteStep }: Props) {
  const [hovered, setHovered] = useState(false);
  const [description, setDescription] = useState<string>(
    step?.description ?? "",
  );
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  if (!step) return null;

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
      className="flex items-center w-full gap-2 py-1 min-h-[36px]"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div className="ml-4 mr-1 text-md min-w-[24px] text-right select-none shrink-0">
        <span className="text-[#888]">{index}.</span>
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
