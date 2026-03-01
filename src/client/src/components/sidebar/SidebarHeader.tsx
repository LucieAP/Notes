import useAuth from "@/shared/hooks/useAuth";
import { NavLink } from "react-router-dom";
import LogoutButton from "../common/buttons/LogoutButton";
import defaultAvatar from "@/components/common/icons/default_avatar.png";

function SidebarHeader() {
  const { user, logout } = useAuth();

  return (
    <div className="flex px-2 pb-5">
      <NavLink
        to="/workspace"
        className="flex items-center gap-2 px-2 py-1 rounded-lg cursor-pointer transition-colors select-none"
      >
        <div className="rounded-full bg-neutral-700 flex items-center justify-center text-white text-xs font-medium">
          {user?.picture ? (
            <img
              className="w-5 h-5 rounded-full"
              src={user.picture}
              alt="avatar"
              onError={(e) => {
                console.log("IMAGE ERROR");
                e.currentTarget.src = defaultAvatar;
              }}
            />
          ) : (
            <div className="w-5 h-5 rounded-full bg-neutral-600" />
          )}
        </div>
        <span className="text-sm font-medium">{user?.name}</span>
      </NavLink>

      <LogoutButton onLogout={logout} />
    </div>
  );
}

export default SidebarHeader;
