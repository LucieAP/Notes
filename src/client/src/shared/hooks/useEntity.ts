import { EntityType } from "../types/entityType";
import useNotes from "./useNotes";
import useRecipes from "./useRecipes";
import useTasks from "./useTasks";

function useEntity(entityType: EntityType) {
  const notes = useNotes();
  const tasks = useTasks();
  const recipes = useRecipes();

  const map = {
    notes: {
      data: notes.notesData,
      isLoading: notes.isLoading,
      create: notes.createNote,
      updateTitle: notes.updateTitle,
      trashItem: notes.trashNote,
      deleteItem: notes.deleteNote,
      isFavorite: (item: any) => item.isPinned,
      toggleFav: notes.togglePin,
    },
    tasks: {
      data: tasks.tasksData,
      isLoading: tasks.isLoading,
      create: tasks.createTask,
      updateTitle: tasks.updateTitle,
      trashItem: tasks.trashTask,
      deleteItem: tasks.deleteTask,
      isFavorite: (item: any) => item.isFavorite,
      toggleFav: tasks.toggleFavorite,
    },
    recipes: {
      data: recipes.recipesData,
      isLoading: recipes.isLoading,
      create: recipes.createRecipe,
      updateTitle: recipes.updateTitle,
      trashItem: recipes.trashRecipe,
      deleteItem: recipes.deleteRecipe,
      isFavorite: (item: any) => item.isFavorite,
      toggleFav: recipes.toggleFavorite,
    },
  };

  return map[entityType];
}

export default useEntity;
