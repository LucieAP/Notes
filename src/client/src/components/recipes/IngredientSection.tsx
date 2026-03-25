import IngredientRow from "./IngredientRow";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import { Unit } from "@/shared/enums/unit";

interface Props {
  recipe?: GetRecipeResponse;
  onIngredientNameChange?: (ingredientId: string, name: string) => void;
  onIngredientQuantityChange?: (ingredientId: string, quantity: number) => void;
  onIngredientUnitChange?: (ingredientId: string, unit: Unit) => void;
  onCreateIngredient?: () => void | Promise<void>;
  onDeleteIngredient?: (ingredientId: string) => void | Promise<void>;
}

function IngredientSection({
  recipe,
  onIngredientNameChange,
  onIngredientQuantityChange,
  onIngredientUnitChange,
  onCreateIngredient,
  onDeleteIngredient,
}: Props) {
  if (!recipe) return null;

  return (
    <section className="w-full">
      <span className="text-[#888] text-sm min-w-[24px] text-right select-none shrink-0 pt-px">
        Ингредиенты:
      </span>

      {recipe.ingredients.map((ingredient, index) => (
        <IngredientRow
          key={ingredient.id}
          index={index + 1}
          ingredient={ingredient}
          onIngredientNameChange={onIngredientNameChange}
          onIngredientQuantityChange={onIngredientQuantityChange}
          onIngredientUnitChange={onIngredientUnitChange}
          onDelete={onDeleteIngredient}
        />
      ))}

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
