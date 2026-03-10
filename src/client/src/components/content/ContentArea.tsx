import { NavLink, Outlet, useParams } from "react-router-dom";
import NoteCard from "../notes/NoteCard";
import useNotes from "@/shared/hooks/useNotes";
import useTasks from "@/shared/hooks/useTasks";
import useRecipes from "@/shared/hooks/useRecipes";
import useActiveEntityType from "@/shared/hooks/useActiveEntityType";
import RecipeCard from "../recipes/RecipeCard";
import TaskCard from "../tasks/TaskCard";

function ContentArea() {
  const { id } = useParams<{ id?: string }>();
  const entityType = useActiveEntityType();
  const { notesData } = useNotes();
  const { tasksData } = useTasks();
  const { recipesData } = useRecipes();

  return (
    <>
      <Outlet />

      {entityType === "notes" && !id && (
        <div className="flex overflow-y-auto w-full justify-center">
          <div className="flex flex-col px-24 py-20 max-w-md w-full justify-center">
            <span className="text-2xl pl-2 font-bold">Notes</span>
            {notesData.map((note) => (
              <NavLink key={note.id} to={`/notes/${note.id}`}>
                <NoteCard note={note} />
              </NavLink>
            ))}
          </div>
        </div>
      )}

      {entityType === "recipes" && !id && (
        <div className="flex overflow-y-auto w-full justify-center">
          <div className="flex flex-col px-24 py-20 max-w-md w-full justify-center">
            <span className="text-2xl pl-2 font-bold">Recipes</span>
            {recipesData.map((recipe) => (
              <NavLink key={recipe.id} to={`/recipes/${recipe.id}`}>
                <RecipeCard recipe={recipe} />
              </NavLink>
            ))}
          </div>
        </div>
      )}

      {entityType === "tasks" && !id && (
        <div className="flex overflow-y-auto w-full justify-center">
          <div className="flex flex-col px-24 py-20 max-w-md w-full justify-center">
            <span className="text-2xl pl-2 font-bold">Tasks</span>
            {tasksData.map((task) => (
              <NavLink key={task.id} to={`/tasks/${task.id}`}>
                <TaskCard task={task} />
              </NavLink>
            ))}
          </div>
        </div>
      )}
    </>
  );
}

export default ContentArea;
