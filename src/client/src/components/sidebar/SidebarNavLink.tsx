import cn from "@/shared/utils/cn";
import { NavLink, useMatch } from "react-router-dom";
import CreateItemButton from "../common/buttons/CreateItemButton";
import useNotes from "@/shared/hooks/useNotes";
import KebabButton from "../common/buttons/KebabButton";

interface Props {
  itemId?: string;
  to: string;
  icon: React.ReactNode;
  label: string;
  showAddButton?: boolean;
}

function SidebarNavLink({
  itemId,
  to,
  icon,
  label,
  showAddButton = true,
}: Props) {
  const match = useMatch(to); // совпадает ли текущий URL с заданным маршрутом
  const isActive = Boolean(match);

  const { createNote } = useNotes();

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

      {itemId && <KebabButton itemId={itemId} />}
      {showAddButton && <CreateItemButton onCreateNote={createNote} />}
    </div>
  );
}

export default SidebarNavLink;
