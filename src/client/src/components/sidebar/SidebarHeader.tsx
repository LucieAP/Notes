import useAuth from "@/shared/hooks/useAuth";
import { NavLink } from "react-router-dom";
import LogoutButton from "../common/buttons/LogoutButton";

function SidebarHeader() {
  const { user, logout } = useAuth();

  return (
    <div className="flex px-2 pb-5">
      <NavLink
        to="/workspace"
        className="flex items-center gap-2 px-2 py-1 rounded-lg cursor-pointer transition-colors select-none"
      >
        <div className="rounded-full bg-neutral-700 flex items-center justify-center text-white text-xs font-medium">
          <img
            className="w-5 h-5 rounded-full"
            src={user?.picture?.toString()}
            alt=""
          />
        </div>
        <span className="text-sm font-medium">{user?.name}</span>
      </NavLink>

      <LogoutButton onLogout={logout} />
    </div>
  );
}

export default SidebarHeader;
