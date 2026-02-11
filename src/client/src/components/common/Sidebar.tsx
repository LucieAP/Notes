import { Link } from "react-router-dom";
import { Outlet } from "react-router-dom";

function Sidebar() {
  return (
    <div className="layout">
      <aside>
        <Link to="/workspace">Workspace</Link>
        <br />
        <Link to="/notes">Notes</Link>
        <br />
        <Link to="/tasks">Tasks</Link>
        <br />
        <Link to="/recipes">Recipes</Link>
      </aside>

      <section className="content">
        <Outlet />
      </section>
    </div>
  );
}

export default Sidebar;
