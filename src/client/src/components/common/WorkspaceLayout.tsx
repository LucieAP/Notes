import { Outlet } from "react-router-dom";
import Header from "./Header";

function WorkspaceLayout() {
  return (
    <>
      <Header />
      <main>
        <Outlet />
      </main>
    </>
  );
}

export default WorkspaceLayout;
