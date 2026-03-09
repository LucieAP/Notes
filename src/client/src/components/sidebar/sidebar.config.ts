import { EntityType } from "@/shared/types/entityType";
import WorkspaceIcon from "../common/icons/WorkspaceIcon";

export interface NavItem {
  to: string;
  icon: React.ComponentType;
  label: string;
  entityType?: EntityType;
  onCreate?: () => Promise<void>; // своя функция для каждого пункта
  children?: ChildNavItem[]; // дочерние элементы
}

export interface ChildNavItem {
  itemId: string;
  to: string;
  label: string;
}

export const mainNavItems: NavItem[] = [
  {
    to: "/workspace",
    icon: WorkspaceIcon,
    label: "Workspace",
  },
];
