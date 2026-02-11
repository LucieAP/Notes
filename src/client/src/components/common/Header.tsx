import { Link } from "react-router-dom";

function HeaderPage() {
  return (
    <>
      <div>
        <h1>Header</h1>
      </div>

      <main>
        <Link to="/workspace">Workspace</Link>
        <br />
        <Link to="/notes">Notes</Link>
        <br />
        <Link to="/tasks">Tasks</Link>
        <br />
        <Link to="/recipes">Recipes</Link>
        <br />
        <Link to="/login">Login</Link>
      </main>
    </>
  );
}

export default HeaderPage;
