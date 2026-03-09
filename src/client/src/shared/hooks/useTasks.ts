import { GetTaskResponse } from "@/features/tasks/task";
import { createContext, useContext } from "react";

interface TasksContextTypes {
  tasksData: GetTaskResponse[];
  fetchTasks: () => Promise<void>;
  createTask: () => Promise<void>;
  getTaskById: (id: string) => Promise<GetTaskResponse | undefined>;
  updateTitle: ({ id, title }: { id: string; title: string }) => Promise<void>;
  updateDescription: ({
    id,
    description,
  }: {
    id: string;
    description: string;
  }) => Promise<void>;
  deleteTask: (id: string) => Promise<void>;
  trashTask: (id: string) => Promise<void>;
  getTrashed: () => Promise<GetTaskResponse[]>;
  restoreTask: (id: string) => Promise<void>;
  favoriteTask: (id: string) => Promise<void>;
  toggleCheckbox: (id: string) => Promise<void>;
}

export const TasksContext = createContext<TasksContextTypes | null>(null);

function useTasks() {
  const tasksContext = useContext(TasksContext);

  if (!tasksContext) {
    throw new Error("useTasks must be used within TasksProvider");
  }

  return tasksContext;
}

export default useTasks;
