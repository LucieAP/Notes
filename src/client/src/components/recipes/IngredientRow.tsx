import { useState } from "react";
import { unitLabels } from "@/shared/enums/unit";
import { GetIngredientResponse } from "@/features/recipes/ingredient";

interface Props {
  index?: number;
  ingredient?: GetIngredientResponse;
  onDelete?: (id: string) => Promise<void>;
}

function IngredientRow({ index = 1, ingredient, onDelete }: Props) {
  const [hovered, setHovered] = useState(false);

  if (!ingredient) return;

  return (
    <section
      className="flex items-center gap-2 py-1 min-h-[36px]"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div className="ml-4 mr-1 text-md min-w-[24px] text-right select-none shrink-0">
        <span className="text-[#888]">{index}.</span>
      </div>

      <input
        type="text"
        className="bg-transparent border-none outline-none text-white text-sm flex-1 placeholder-[#555]"
        placeholder="Введите ингредиент"
        defaultValue={ingredient.name}
      />

      <input
        type="number"
        className="no-arrows bg-transparent border-none outline-none border border-[#444] text-white text-sm w-12 text-right placeholder-[#555]"
        placeholder="0"
        defaultValue={ingredient.quantity}
      />

      <select
        className="bg-[#333] border border-[#444] rounded text-white text-sm px-1.5 py-0.5 outline-none cursor-pointer"
        defaultValue={ingredient.unit}
      >
        {Object.values(unitLabels).map((label) => (
          <option key={label} value={label}>
            {label}
          </option>
        ))}
      </select>

      <button
        className="text-[#666] hover:text-white text-xs px-1 py-0.5 rounded bg-transparent border-none cursor-pointer transition-all duration-150 shrink-0"
        style={{ opacity: hovered ? 1 : 0 }}
        onClick={() => onDelete?.(ingredient.id)}
        aria-label="Удалить ингредиент"
      >
        ✕
      </button>
    </section>
  );
}

export default IngredientRow;
