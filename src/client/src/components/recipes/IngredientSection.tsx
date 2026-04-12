import IngredientRow from "./IngredientRow";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import { Unit } from "@/shared/enums/unit";
import { DragDropProvider, type DragEndEvent } from "@dnd-kit/react";
import { isSortableOperation } from "@dnd-kit/react/sortable";

interface Props {
  recipe?: GetRecipeResponse;
  onIngredientNameChange?: (ingredientId: string, name: string) => void;
  onIngredientQuantityChange?: (ingredientId: string, quantity: number) => void;
  onIngredientUnitChange?: (ingredientId: string, unit: Unit) => void;
  onCreateIngredient?: () => void | Promise<void>;
  onDeleteIngredient?: (ingredientId: string) => void | Promise<void>;
  onReorderIngredients?: (
    orderedIngredientIds: string[],
  ) => void | Promise<void>;
}

function IngredientSection({
  recipe,
  onIngredientNameChange,
  onIngredientQuantityChange,
  onIngredientUnitChange,
  onCreateIngredient,
  onDeleteIngredient,
  onReorderIngredients,
}: Props) {
  if (!recipe) return null;

  const handleDragEnd: DragEndEvent = async (event) => {
    const { operation, canceled } = event;
    if (canceled || !isSortableOperation(operation)) return;

    const from = operation.source?.initialIndex; // индекс элемента в момент начала перетаскивания
    const to = operation.source?.index; // индекс этого же элемента в момент окончания drag (куда он “встал” после сортировки)

    if (from == null || to == null || from === to) return;

    const next = [...recipe.ingredients]; // дублируем чтобы избежать мутации
    const [moved] = next.splice(from, 1); // возвращает массив удаленных элементов
    next.splice(to, 0, moved); // вставляет элемент moved в массив next по индексу targetIndex

    const orderedIngredientIds = next.map((ingredient) => ingredient.id);

    console.log("orderedIngredientIds", orderedIngredientIds);

    await onReorderIngredients?.(orderedIngredientIds);
  };

  return (
    <section className="w-full">
      <span className="text-[#888] text-sm min-w-[24px] text-right select-none shrink-0 pt-px">
        Ингредиенты:
      </span>

      <DragDropProvider onDragEnd={handleDragEnd}>
        {recipe.ingredients.map((ingredient, index) => (
          <IngredientRow
            key={ingredient.id}
            index={index}
            ingredient={ingredient}
            onIngredientNameChange={onIngredientNameChange}
            onIngredientQuantityChange={onIngredientQuantityChange}
            onIngredientUnitChange={onIngredientUnitChange}
            onDelete={onDeleteIngredient}
          />
        ))}
      </DragDropProvider>

      <div className="flex gap-1 mt-2">
        <button
          onClick={onCreateIngredient}
          className="inline-flex items-center gap-1.5 text-[#c5c5c5] hover:text-white bg-[#2a2a2a]/70 hover:bg-[#353535] border border-[#3a3a3a] hover:border-[#4a4a4a] text-xs px-2.5 py-1.5 rounded-md cursor-pointer transition-all duration-150 active:scale-[0.98] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#444]"
        >
          <span className="inline-flex items-center justify-center w-4 h-4 rounded-full bg-[#2b2b2b] text-[11px] leading-none">
            +
          </span>
          ингредиент
        </button>

        {/* <button className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150">
          <span className="text-sm leading-none">+</span> подгруппа
        </button> */}
      </div>
    </section>
  );
}

export default IngredientSection;
