import RecipesEditor from "./RecipesEditor";
import { useParams } from "react-router-dom";
import Spinner from "../common/icons/Spinner";
import useRecipeEditor from "@/shared/hooks/useRecipeEditor";

function RecipesEditorPage() {
  const { id } = useParams<{ id: string }>();
  if (!id) return null;

  const { recipe, isLoading, actions } = useRecipeEditor(id);

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
