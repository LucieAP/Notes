import api from "@/shared/api/axios";
import {
  CreateTaskRequest,
  GetTaskResponse,
  ToggleCheckboxResponse,
  ToggleFavoriteResponse,
  UpdateTaskDescriptionRequest,
  UpdateTaskResponse,
  UpdateTaskTitleRequest,
} from "./task";

export const tasksApi = {
  getAll(): Promise<GetTaskResponse[]> {
    return api.get("/tasks");
  },

  getTaskById(id: string): Promise<GetTaskResponse> {
    return api.get(`/tasks/${id}`);
  },

  create(data: CreateTaskRequest): Promise<GetTaskResponse> {
    return api.post("/tasks/create", data);
  },

  toggleFavorite(id: string): Promise<ToggleFavoriteResponse> {
    return api.patch(`tasks/${id}/favorite`);
  },

  toggleCheckbox(id: string): Promise<ToggleCheckboxResponse> {
    return api.patch(`tasks/${id}/checkbox`);
  },

  updateTitle({
    id,
    title,
  }: UpdateTaskTitleRequest): Promise<UpdateTaskResponse> {
    return api.patch(`tasks/${id}`, { title });
  },

  updateDescription({
    id,
    description,
  }: UpdateTaskDescriptionRequest): Promise<UpdateTaskResponse> {
    return api.patch(`tasks/${id}`, { description });
  },

  delete(id: string) {
    return api.delete(`tasks/delete/${id}`);
  },

  trash(id: string) {
    return api.patch(`tasks/${id}/trash`);
  },

  restore(id: string) {
    return api.patch(`tasks/${id}/restore`);
  },

  getTrashed(): Promise<GetTaskResponse[]> {
    return api.get("/tasks/trashed");
  },
};
