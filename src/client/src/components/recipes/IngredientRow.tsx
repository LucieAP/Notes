import { useEffect, useRef, useState } from "react";
import { Unit, unitLabels } from "@/shared/enums/unit";
import { GetIngredientResponse } from "@/features/recipes/ingredient";
import { useSortable } from "@dnd-kit/react/sortable";
import DragAndDropButton from "../common/buttons/DragAndDropButton";

interface Props {
  index: number;
  ingredient?: GetIngredientResponse;
  onIngredientNameChange?: (ingredientId: string, name: string) => void;
  onIngredientQuantityChange?: (ingredientId: string, quantity: number) => void;
  onIngredientUnitChange?: (ingredientId: string, unit: Unit) => void;
  onDelete?: (id: string) => void | Promise<void>;
}

function IngredientRow({
  index,
  ingredient,
  onIngredientNameChange,
  onIngredientQuantityChange,
  onIngredientUnitChange,
  onDelete,
}: Props) {
  if (!ingredient) return;

  const [hovered, setHovered] = useState(false);
  const [name, setName] = useState(ingredient.name ?? "");
  const [quantity, setQuantity] = useState(
    ingredient.quantity?.toString() ?? "",
  );
  const [unit, setUnit] = useState<Unit>(ingredient.unit ?? Unit.Default);
  const displayIndex = index + 1;
  const handleRef = useRef<HTMLButtonElement | null>(null);
  const sortable = useSortable({
    id: ingredient.id,
    index: index,
    handle: handleRef,
  });

  useEffect(() => {
    setName(ingredient.name ?? "");
  }, [ingredient.name]);

  useEffect(() => {
    setQuantity(ingredient.quantity?.toString() ?? "");
  }, [ingredient.quantity]);

  useEffect(() => {
    setUnit(ingredient.unit ?? Unit.Default);
  }, [ingredient.unit]);

  return (
    <section
      ref={sortable.ref}
      className="flex items-center gap-2 py-1 min-h-[36px]"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <DragAndDropButton ref={handleRef} isDragging={sortable.isDragging} />

      <div className="mr-1 text-md min-w-[24px] text-right select-none shrink-0">
        <span className="text-[#888]">{displayIndex}.</span>
      </div>

      <input
        type="text"
        className="bg-transparent border-none outline-none text-[#888] text-sm flex-1 placeholder-[#555]"
        placeholder="Введите ингредиент"
        value={name}
        onChange={(e) => {
          const nextValue = e.target.value ?? "";
          setName(nextValue);
          onIngredientNameChange?.(ingredient.id, nextValue);
        }}
      />

      <input
        type="number"
        className="no-arrows bg-transparent border-none outline-none border border-[#444] text-[#888] text-sm w-12 text-right placeholder-[#555]"
        placeholder="0"
        value={quantity}
        onChange={(e) => {
          const nextValue = e.target.value ?? "";
          setQuantity(nextValue);
          const parsed = Number(nextValue);
          if (Number.isNaN(parsed)) return;
          onIngredientQuantityChange?.(ingredient.id, parsed);
        }}
      />

      <select
        className="bg-[#333] border border-[#444] rounded text-[#888] text-sm px-1.5 py-0.5 outline-none cursor-pointer"
        value={unit}
        onChange={(e) => {
          const nextUnit = Number(e.target.value) as Unit;
          setUnit(nextUnit);
          onIngredientUnitChange?.(ingredient.id, nextUnit);
        }}
      >
        {Object.entries(unitLabels).map(([unitKey, label]) => (
          <option key={unitKey} value={unitKey}>
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
