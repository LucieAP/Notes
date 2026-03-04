import "../styles/App.css";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";

import WorkspaceLayout from "../components/layout/WorkspaceLayout";
import WorkspacePage from "../components/workspace/WorkspacePage";
import NotesEditorPage from "../components/notes/NotesEditorPage";
import TasksPage from "../components/tasks/TasksPage";
import RecipesPage from "../components/recipes/RecipesPage";
import NotFoundPage from "../components/common/pages/NotFoundPage";
import ContentArea from "@/components/content/ContentArea";
import LoginPage from "@/components/auth/LoginPage";
import CallbackPage from "@/components/auth/CallbackPage";
import AuthProvider from "./providers/AuthProvider";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";
import NotesProvider from "./providers/NotesProvider";

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/callback" element={<CallbackPage />} />

          <Route
            path="/"
            element={
              <ProtectedRoute>
                <NotesProvider>
                  <WorkspaceLayout />
                </NotesProvider>
              </ProtectedRoute>
            }
          >
            <Route element={<ContentArea />}>
              <Route index element={<Navigate to="workspace" replace />} />
              <Route path="workspace" element={<WorkspacePage />} />

              <Route path="notes">
                <Route path=":id" element={<NotesEditorPage />} />
              </Route>
              <Route path="tasks" element={<TasksPage />} />
              <Route path="recipes" element={<RecipesPage />} />
            </Route>
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
