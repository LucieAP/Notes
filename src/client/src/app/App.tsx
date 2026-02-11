import "../styles/App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import WorkspaceLayout from "../components/common/WorkspaceLayout";
import WorkspacePage from "../components/workspace/WorkspacePage";
import NotesPage from "../components/notes/NotesPage";
import TasksPage from "../components/tasks/TasksPage";
import RecipesPage from "../components/recipes/RecipesPage";
import LoginPage from "../components/auth/LoginPage";
import NotFoundPage from "../components/common/NotFoundPage";
import Sidebar from "@/components/common/SideBar";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<WorkspaceLayout />}>
          <Route element={<Sidebar />}>
            <Route path="workspace" element={<WorkspacePage />} />
            <Route path="notes" element={<NotesPage />} />
            <Route path="tasks" element={<TasksPage />} />
            <Route path="recipes" element={<RecipesPage />} />
          </Route>
        </Route>

        <Route path="/login" element={<LoginPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </Router>
  );
}

export default App;
