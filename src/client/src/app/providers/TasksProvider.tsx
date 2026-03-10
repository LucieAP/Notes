import { tasksApi } from "@/features/tasks/api";
import { GetTaskResponse } from "@/features/tasks/task";
import { TasksContext } from "@/shared/hooks/useTasks";
import { useCallback, useEffect, useMemo, useState } from "react";

function TasksProvider({ children }: { children: React.ReactNode }) {
  const [tasksData, setTasksData] = useState<GetTaskResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchTasks = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await tasksApi.getAll();
      setTasksData(data ?? []);
      console.log("fetched tasks:", data);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  const createTask = useCallback(async () => {
    try {
      const createdTask = await tasksApi.create({
        title: "New Task",
        description: null,
        checkBox: false,
        isFavorite: false,
        backgroundColor: 0,
      });

      setTasksData((prev) => [...prev, createdTask]);
      console.log(`createdTask: ${createdTask}`);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const getTaskById = useCallback(
    async (id: string) => {
      try {
        if (!id) return;

        const cachedTask = tasksData.find((task) => task.id === id);
        if (cachedTask) return cachedTask;

        const task = await tasksApi.getTaskById(id);
        setTasksData((prev) => {
          const existingIndex = prev.findIndex((item) => item.id === task.id);
          if (existingIndex >= 0) {
            const next = [...prev];
            next[existingIndex] = task;
            return next;
          }
          return [...prev, task];
        });
        return task;
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [tasksData],
  );

  const updateTitle = useCallback(
    async ({ id, title }: { id: string; title: string }) => {
      try {
        if (!id) return;
        await tasksApi.updateTitle({ id, title });
        fetchTasks();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchTasks],
  );

  const updateDescription = useCallback(
    async ({ id, description }: { id: string; description: string }) => {
      try {
        if (!id) return;
        await tasksApi.updateDescription({ id, description });
        fetchTasks();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchTasks],
  );

  const deleteTask = useCallback(
    async (id: string) => {
      try {
        if (!id) return;
        await tasksApi.delete(id);
        fetchTasks();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchTasks],
  );

  const trashTask = useCallback(
    async (id: string) => {
      try {
        if (!id) return;
        await tasksApi.trash(id);
        fetchTasks();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchTasks],
  );

  const restoreTask = useCallback(
    async (id: string) => {
      try {
        if (!id) return;
        await tasksApi.restore(id);
        fetchTasks();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchTasks],
  );

  const getTasksTrashed = useCallback(async () => {
    try {
      const data = await tasksApi.getTrashed();
      return data ?? [];
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
      throw err;
    }
  }, []);

  const toggleFavorite = useCallback(async (id: string) => {
    try {
      const data = await tasksApi.toggleFavorite(id);
      setTasksData((prev) =>
        prev.map((task) =>
          task.id === id ? { ...task, isFavorite: data.isFavorite } : task,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const toggleCheckbox = useCallback(async (id: string) => {
    try {
      const data = await tasksApi.toggleCheckbox(id);
      setTasksData((prev) =>
        prev.map((task) =>
          task.id === id ? { ...task, checkBox: data.checkBox } : task,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const value = useMemo(() => {
    return {
      tasksData,
      isLoading,
      fetchTasks,
      createTask,
      getTaskById,
      updateTitle,
      updateDescription,
      deleteTask,
      trashTask,
      getTasksTrashed,
      restoreTask,
      toggleFavorite,
      toggleCheckbox,
    };
  }, [
    tasksData,
    isLoading,
    fetchTasks,
    createTask,
    getTaskById,
    updateTitle,
    updateDescription,
    deleteTask,
    trashTask,
    getTasksTrashed,
    restoreTask,
    toggleFavorite,
    toggleCheckbox,
  ]);

  return <TasksContext value={value}>{children}</TasksContext>;
}

export default TasksProvider;
