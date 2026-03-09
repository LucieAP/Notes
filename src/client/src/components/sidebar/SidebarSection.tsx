import useNotes from "@/shared/hooks/useNotes";
import DropdownIcon from "../common/icons/DropdownIcon";
import NoteIcon from "../common/icons/NoteIcon";
import { NavItem } from "./sidebar.config";
import SidebarNavLink from "./SidebarNavLink";
import { useMemo, useState } from "react";

import useTasks from "@/shared/hooks/useTasks";
import useRecipes from "@/shared/hooks/useRecipes";
import RecipeIcon from "../common/icons/RecipeIcon";
import TaskIcon from "../common/icons/TaskIcon";

interface Props {
  navItems?: NavItem[];
  title: string;
  defaultOpen?: boolean;
  type?: "private" | "favorite";
}

function SidebarSection({ navItems, title, defaultOpen = true, type }: Props) {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  const { notesData, isLoading: notesLoading } = useNotes();
  const { tasksData, isLoading: tasksLoading } = useTasks();
  const { recipesData, isLoading: recipesLoading } = useRecipes();

  const isLoading = notesLoading || tasksLoading || recipesLoading;

  const favoriteNotes = useMemo(
    () => (type === "favorite" ? notesData.filter((n) => n.isPinned) : []),
    [notesData, type],
  );

  const favoriteTasks = useMemo(
    () => (type === "favorite" ? tasksData.filter((t) => t.isFavorite) : []),
    [tasksData, type],
  );

  const favoriteRecipes = useMemo(
    () => (type === "favorite" ? recipesData.filter((r) => r.isFavorite) : []),
    [recipesData, type],
  );

  return (
    <div className="flex flex-col mt-5">
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex rounded-lg hover:bg-neutral-700 px-1 py-1 space-x-0.5 text-xs cursor-pointer"
      >
        <span>{title}</span>
        <DropdownIcon isOpen={isOpen} />
      </button>

      {isOpen && type === "private" && (
        <div className="flex flex-col">
          {navItems?.map((item) => {
            if (!item) return null;
            return (
              <div key={item.to} className="flex flex-col">
                <SidebarNavLink
                  to={item.to}
                  icon={<item.icon />}
                  label={item.label}
                  onCreate={item.onCreate}
                />
                {/* Дочерние элементы */}
                {item.children?.map((child) => (
                  <div key={child.to} className="pl-4">
                    <SidebarNavLink
                      itemId={child.itemId}
                      to={child.to}
                      icon={<item.icon />}
                      label={child.label}
                    />
                  </div>
                ))}
              </div>
            );
          })}
        </div>
      )}

      {isOpen && type === "favorite" && (
        <div className="flex flex-col mt-1">
          {isLoading ? (
            <p className="text-xs text-neutral-500 px-2 py-2">Loading...</p>
          ) : favoriteNotes.length === 0 &&
            favoriteTasks.length === 0 &&
            favoriteRecipes.length === 0 ? (
            <p className="text-xs text-neutral-500 px-2 py-2">
              No favorites yet
            </p>
          ) : (
            <>
              {favoriteNotes.map((note) => (
                <SidebarNavLink
                  key={note.id}
                  itemId={note.id}
                  to={`/notes/${note.id}`}
                  icon={<NoteIcon />}
                  label={note.title || "Untitled"}
                />
              ))}
              {favoriteTasks.map((task) => (
                <SidebarNavLink
                  key={task.id}
                  itemId={task.id}
                  to={`/tasks/${task.id}`}
                  icon={<TaskIcon />}
                  label={task.title || "Untitled"}
                />
              ))}
              {favoriteRecipes.map((recipe) => (
                <SidebarNavLink
                  key={recipe.id}
                  itemId={recipe.id}
                  to={`/recipes/${recipe.id}`}
                  icon={<RecipeIcon />}
                  label={recipe.title || "Untitled"}
                />
              ))}
            </>
          )}
        </div>
      )}
    </div>
  );
}

export default SidebarSection;
