import { Navigate, NavLink } from "react-router-dom";

function SidebarHeader() {
  return (
    <div className="flex flex-col px-2 pb-5">
      <NavLink
        to="/workspace"
        className="flex items-center gap-2 px-2 py-1 rounded-lg cursor-pointer transition-colors select-none"
      >
        <div className="w-5 h-5 rounded bg-neutral-700 flex items-center justify-center text-white text-xs font-medium">
          P
        </div>
        <span className="text-sm font-medium">Profile</span>
      </NavLink>
    </div>
  );
}

export default SidebarHeader;
