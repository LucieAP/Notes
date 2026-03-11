import { useEffect, useState } from "react";
import RecipesEditor from "./RecipesEditor";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import { useParams } from "react-router-dom";
import useRecipes from "@/shared/hooks/useRecipes";
import Spinner from "../common/icons/Spinner";

function RecipesEditorPage() {
  const { id } = useParams<{ id: string }>();
  const [recipe, setRecipe] = useState<GetRecipeResponse | null>();

  const { getRecipeById, isLoading } = useRecipes();

  useEffect(() => {
    if (!id) return;

    (async () => {
      const fetchedRecipe = await getRecipeById(id);
      if (fetchedRecipe) {
        setRecipe(fetchedRecipe);
        console.log("recipe: ", fetchedRecipe);
      }
    })();
  }, [id, getRecipeById]);

  return (
    <>
      {!isLoading ? (
        recipe ? (
          <RecipesEditor key={recipe.id} />
        ) : (
          <div>Recipe not found</div>
        )
      ) : (
        <Spinner />
      )}
    </>
  );
}

export default RecipesEditorPage;
