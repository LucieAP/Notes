import cn from "@/shared/utils/cn";
import { NavLink } from "react-router-dom";

interface Props {
  to: string;
  icon: React.ReactNode;
  label: string;
}

function SidebarNavLink({ to, icon, label }: Props) {
  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        cn(
          "flex rounded-lg px-2 py-1 space-x-2",
          isActive ? "bg-neutral-700" : "hover:bg-neutral-700",
        )
      }
    >
      {icon}
      <span>{label}</span>
    </NavLink>
  );
}

export default SidebarNavLink;
