import { Outlet } from "react-router-dom";
import Header from "./Header";
import Sidebar from "./Sidebar";

function WorkspaceLayout() {
  return (
    <>
      <div className="h-screen w-screen flex flex-col overflow-hidden">
        <div className="flex flex-1 overflow-hidden">
          <Sidebar />
          <Outlet />
        </div>
      </div>
    </>
  );
}

export default WorkspaceLayout;
