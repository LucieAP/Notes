import SidebarHeader from "./SidebarHeader";
import SidebarNavLink from "./SidebarNavLink";
import { mainNavItems, NavItem } from "./sidebar.config";
import SidebarSection from "./SidebarSection";
import SidebarFooter from "./SidebarFooter";
import SearchButton from "../common/buttons/SearchButton";
import useResizableSidebar from "@/shared/hooks/useResizableSidebar";
import { useEffect, useMemo } from "react";
import NoteIcon from "../common/icons/NoteIcon";
import TaskIcon from "../common/icons/TaskIcon";
import RecipeIcon from "../common/icons/RecipeIcon";
import useNotes from "@/shared/hooks/useNotes";
import useRecipes from "@/shared/hooks/useRecipes";
import useTasks from "@/shared/hooks/useTasks";

const SIDEBAR_WIDTH_KEY = "sidebar-width";
const DEFAULT_WIDTH = 240;

function Sidebar() {
  const raw = Number(localStorage.getItem(SIDEBAR_WIDTH_KEY));
  const savedWidth = Number.isFinite(raw) ? raw : DEFAULT_WIDTH; // isFinite - если число и не NaN
  const { notesData, createNote } = useNotes();
  const { tasksData, createTask } = useTasks();
  const { recipesData, createRecipe } = useRecipes();

  const privateNavItems = useMemo<NavItem[]>(
    () => [
      {
        to: "/notes",
        icon: NoteIcon,
        label: "Notes",
        entityType: "notes",
        onCreate: createNote,
        children: notesData.map((note) => ({
          itemId: note.id,
          to: `/notes/${note.id}`,
          label: note.title || "Untitled",
        })),
      },
      {
        to: "/tasks",
        icon: TaskIcon,
        label: "Tasks",
        entityType: "tasks",
        onCreate: createTask,
        children: tasksData.map((task) => ({
          itemId: task.id,
          to: `/tasks/${task.id}`,
          label: task.title || "Untitled",
        })),
      },
      {
        to: "/recipes",
        icon: RecipeIcon,
        label: "Recipes",
        entityType: "recipes",
        onCreate: createRecipe,
        children: recipesData.map((recipe) => ({
          itemId: recipe.id,
          to: `/recipes/${recipe.id}`,
          label: recipe.title || "Untitled",
        })),
      },
    ],
    [notesData, tasksData, recipesData, createNote, createTask, createRecipe],
  );

  const [width, setWidth, onMouseDown] = useResizableSidebar({
    initialWidth: savedWidth,
  });

  useEffect(() => {
    localStorage.setItem(SIDEBAR_WIDTH_KEY, width.toString());
  }, [width]);

  return (
    <div
      style={{ width }}
      className="relative h-screen border-r border-neutral-800 shrink-0 bg-neutral-800 text-[rgba(186,186,182,1)]"
    >
      {/* Content */}
      <div className="flex flex-1 flex-col h-full p-2 gap-0.5 overflow-hidden">
        <SidebarHeader />
        <div className="flex flex-col px-1 rounded select-none text-sm transition-colors">
          {mainNavItems.map((item) => (
            <SidebarNavLink
              key={item.to}
              to={item.to}
              icon={<item.icon />}
              label={item.label}
            />
          ))}

          <SearchButton />

          <SidebarSection
            navItems={privateNavItems}
            title="Private"
            type="private"
          />
          <SidebarSection title="Favorite" type="favorite" />
        </div>
        <SidebarFooter />
      </div>

      {/* Resize Handle */}
      <div
        onMouseDown={onMouseDown}
        onDoubleClick={() => setWidth(DEFAULT_WIDTH)}
        className="absolute top-0 right-0 h-full w-1 cursor-col-resize hover:bg-neutral-700 active:bg-neutral-600"
      />
    </div>
  );
}

export default Sidebar;
