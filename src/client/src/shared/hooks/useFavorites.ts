import NoteIcon from "@/components/common/icons/NoteIcon";
import { EntityType } from "../types/entityType";
import useNotes from "./useNotes";
import useRecipes from "./useRecipes";
import useTasks from "./useTasks";
import TaskIcon from "@/components/common/icons/TaskIcon";
import RecipeIcon from "@/components/common/icons/RecipeIcon";

function useFavorites() {
  const { notesData, isLoading: notesLoading } = useNotes();
  const { tasksData, isLoading: tasksLoading } = useTasks();
  const { recipesData, isLoading: recipesLoading } = useRecipes();
  const isLoading = notesLoading || tasksLoading || recipesLoading;

  const favorites = [
    ...notesData
      .filter((n) => n.isPinned)
      .map((n) => ({
        id: n.id,
        title: n.title,
        entityType: "notes" as EntityType,
        icon: NoteIcon,
      })),
    ...tasksData
      .filter((t) => t.isFavorite)
      .map((t) => ({
        id: t.id,
        title: t.title,
        entityType: "tasks" as EntityType,
        icon: TaskIcon,
      })),
    ...recipesData
      .filter((r) => r.isFavorite)
      .map((r) => ({
        id: r.id,
        title: r.title,
        entityType: "recipes" as EntityType,
        icon: RecipeIcon,
      })),
  ];

  return { favorites, isLoading };
}

export default useFavorites;
