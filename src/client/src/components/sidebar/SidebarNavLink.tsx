import cn from "@/shared/utils/cn";
import { NavLink } from "react-router-dom";
import AddItemButton from "../common/buttons/AddItemButton";

interface Props {
  to: string;
  icon: React.ReactNode;
  label: string;
  showAddButton?: boolean;
}

function SidebarNavLink({ to, icon, label, showAddButton = true }: Props) {
  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        cn(
          "flex group items-center w-full rounded-lg px-2 py-1 space-x-2",
          isActive ? "bg-neutral-700" : "hover:bg-neutral-700",
        )
      }
    >
      {icon}
      <span>{label}</span>
      <AddItemButton enabled={showAddButton} />
    </NavLink>
  );
}

export default SidebarNavLink;
