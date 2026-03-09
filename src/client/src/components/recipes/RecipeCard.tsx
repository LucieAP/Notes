import { GetRecipeResponse } from "@/features/recipes/recipe";
import RecipeIcon from "../common/icons/RecipeIcon";

interface Props {
  recipe: GetRecipeResponse;
}

function RecipeCard({ recipe }: Props) {
  return (
    <div className="flex space-x-1 p-2 hover:bg-neutral-600 rounded-lg">
      <RecipeIcon />
      <span className="select-none underline decoration-neutral-600 underline-offset-3">
        {recipe.title}
      </span>
    </div>
  );
}

export default RecipeCard;

