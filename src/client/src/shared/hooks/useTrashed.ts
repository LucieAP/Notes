import { EntityType } from "../types/entityType";
import useNotes from "./useNotes";
import useRecipes from "./useRecipes";
import useTasks from "./useTasks";

export interface TrashedItem {
  id: string;
  title: string;
  entityType: EntityType;
}

function useTrashed() {
  const { deleteNote, restoreNote, getNotesTrashed } = useNotes();
  const { deleteTask, restoreTask, getTasksTrashed } = useTasks();
  const { deleteRecipe, restoreRecipe, getRecipesTrashed } = useRecipes();

  const fetchTrashedItems = async (): Promise<TrashedItem[]> => {
    const [notes, tasks, recipes] = await Promise.all([
      getNotesTrashed(),
      getTasksTrashed(),
      getRecipesTrashed(),
    ]);

    return [
      ...notes.map((n) => ({
        id: n.id,
        title: n.title,
        entityType: "notes" as EntityType,
      })),
      ...tasks.map((t) => ({
        id: t.id,
        title: t.title,
        entityType: "tasks" as EntityType,
      })),

      ...recipes.map((r) => ({
        id: r.id,
        title: r.title,
        entityType: "recipes" as EntityType,
      })),
    ];
  };

  const deleteItem = async (id: string, entityType: EntityType) => {
    const map = {
      notes: deleteNote,
      tasks: deleteTask,
      recipes: deleteRecipe,
    };

    return map[entityType](id);
  };

  const restoreItem = async (id: string, entityType: EntityType) => {
    const map = {
      notes: restoreNote,
      tasks: restoreTask,
      recipes: restoreRecipe,
    };

    return map[entityType](id);
  };

  return { fetchTrashedItems, deleteItem, restoreItem };
}
export default useTrashed;
