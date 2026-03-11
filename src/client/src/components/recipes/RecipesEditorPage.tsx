import { useCallback, useEffect, useRef, useState } from "react";
import RecipesEditor from "./RecipesEditor";
import { GetRecipeResponse } from "@/features/recipes/recipe";
import { useParams } from "react-router-dom";
import useRecipes from "@/shared/hooks/useRecipes";
import Spinner from "../common/icons/Spinner";

function RecipesEditorPage() {
  const { id } = useParams<{ id: string }>();
  const [recipe, setRecipe] = useState<GetRecipeResponse | null>();
  const saveTimeoutRef = useRef<number | null>(null);

  const { getRecipeById, isLoading, updateTitle } = useRecipes();

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

  const handleTitleChange = useCallback(
    (title: string) => {
      if (!id) return;

      // очистка предыдущего таймера
      if (saveTimeoutRef.current) {
        window.clearTimeout(saveTimeoutRef.current);
      }

      saveTimeoutRef.current = window.setTimeout(async () => {
        try {
          await updateTitle({ id, title });
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000); // 1 секунда паузы после ввода
    },
    [id, updateTitle],
  );

  return (
    <>
      {!isLoading ? (
        recipe ? (
          <RecipesEditor
            key={recipe.id}
            recipeTitle={recipe.title}
            onTitleChange={handleTitleChange}
          />
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
