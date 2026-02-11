import { Outlet } from "react-router-dom";
import Header from "./Header";

function WorkspaceLayout() {
  return (
    <>
      <Header />
      <Outlet />
    </>
  );
}

export default WorkspaceLayout;
