import cn from "@/shared/utils/cn";
import { NavLink, useMatch } from "react-router-dom";
import AddItemButton from "../common/buttons/AddItemButton";

interface Props {
  to: string;
  icon: React.ReactNode;
  label: string;
  showAddButton?: boolean;
  onAdd?: () => void;
}

function SidebarNavLink({
  to,
  icon,
  label,
  showAddButton = true,
  onAdd,
}: Props) {
  const match = useMatch(to); // совпадает ли текущий URL с заданным маршрутом
  const isActive = Boolean(match);

  return (
    <div
      className={cn(
        "group flex items-center rounded-lg px-2 py-1",
        isActive ? "bg-neutral-700" : "hover:bg-neutral-700",
      )}
    >
      <NavLink to={to} className="flex flex-1 items-center space-x-2">
        {icon}
        <span>{label}</span>
      </NavLink>

      {showAddButton && <AddItemButton onAdd={onAdd} />}
    </div>
  );
}

export default SidebarNavLink;
