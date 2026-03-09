import { BackgroundColor } from "@/shared/enums/backgroundColor";
import { User } from "@/shared/types/user";
import { TaskGroup } from "./taskGroup";

export interface GetTaskResponse {
  id: string;
  title: string;
  description: string | null;
  checkBox: boolean;
  isFavorite: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  backgroundColor: BackgroundColor;
  isDeleted: boolean;
  deletedAt: string | null;
  taskGroup: TaskGroup | null;
  createdBy: User;
}

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
  checkBox: boolean;
  isFavorite: boolean;
  backgroundColor: number;
  taskGroupId?: string | null;
}

export interface CreateTaskResponse {
  id: string;
  title: string;
  description: string | null;
  checkBox: boolean;
  isFavorite: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  backgroundColor: BackgroundColor;
  isDeleted: boolean;
  deletedAt: string | null;
  taskGroupId: string | null;
  createdBy: string;
}

export interface ToggleFavoriteResponse {
  id: string;
  isFavorite: boolean;
  lastModifiedAt: string;
}

export interface ToggleCheckboxResponse {
  id: string;
  checkBox: boolean;
  lastModifiedAt: string;
}

export interface UpdateTaskTitleRequest {
  id: string;
  title: string;
}

export interface UpdateTaskDescriptionRequest {
  id: string;
  description: string;
}

export interface UpdateTaskResponse {
  id: string;
  title: string | null;
  description: string | null;
  lastModifiedAt: string;
  wasUpdated: boolean;
}
