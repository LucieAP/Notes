import WorkspaceIcon from "../common/icons/WorkspaceIcon";
import NoteIcon from "../common/icons/NoteIcon";
import RecipeIcon from "../common/icons/RecipeIcon";
import TaskIcon from "../common/icons/TaskIcon";

export interface NavItem {
  to: string;
  icon: React.ComponentType;
  label: string;
}

export const mainNavItems: NavItem[] = [
  { to: "/workspace", icon: WorkspaceIcon, label: "Workspace" },
];

export const privateNavItems: NavItem[] = [
  { to: "/notes", icon: NoteIcon, label: "Notes" },
  { to: "/tasks", icon: TaskIcon, label: "Tasks" },
  { to: "/recipes", icon: RecipeIcon, label: "Recipes" },
];
