import IngredientRow from "./IngredientRow";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import useRecipes from "@/shared/hooks/useRecipes";

interface Props {
  recipe?: GetRecipeResponse;
}

function IngredientSection({ recipe }: Props) {
  const { createIngredient, deleteIngredient } = useRecipes();

  if (!recipe) {
    return null;
  }

  const handleCreateIngredient = async () => {
    try {
      if (!recipe.id) return;
      await createIngredient(recipe.id);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  };

  const handleDeleteIngredient = async (id: string) => {
    try {
      if (!id) return;
      await deleteIngredient(id);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  };

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
          onDelete={handleDeleteIngredient}
        />
      ))}

      <div className="flex gap-1 mt-2">
        <button
          onClick={handleCreateIngredient}
          className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150"
        >
          <span>+</span> ингредиент
        </button>

        <button className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150">
          <span className="text-sm leading-none">+</span> подгруппа
        </button>
      </div>
    </section>
  );
}

export default IngredientSection;
