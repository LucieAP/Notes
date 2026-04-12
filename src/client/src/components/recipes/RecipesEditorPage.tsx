import RecipesEditor from "./RecipesEditor";
import { useParams } from "react-router-dom";
import Spinner from "../common/icons/Spinner";
import useRecipeEditor from "@/shared/hooks/useRecipeEditor";

function RecipesEditorPage() {
  const { id: recipeId } = useParams<{ id: string }>();
  if (!recipeId) return null;

  const { recipe, isLoading, actions } = useRecipeEditor(recipeId);

  return (
    <>
      {!isLoading ? (
        recipe ? (
          <RecipesEditor
            key={recipe.id}
            recipe={recipe}
            actions={actions}
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
