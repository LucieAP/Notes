import { BackgroundColor } from "@/shared/enums/backgroundColor";
import { NoteGroup } from "./noteGroup";
import { User } from "@/shared/types/user";

/** DTO для получения всех заметок (GetNoteResponse на сервере) */
export interface GetNoteResponse {
  id: string;
  title: string;
  content: string | null;
  isPinned: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  backgroundColor: BackgroundColor;
  isDeleted: boolean;
  deletedAt: string | null;
  noteGroup: NoteGroup | null;
  createdBy: User;
}

/** DTO для создания заметки (CreateNoteRequest на сервере) */
export interface CreateNoteRequest {
  title: string;
  content?: string | null;
  isPinned: boolean;
  backgroundColor: number;
  noteGroupId?: string | null;
}

export interface CreateNoteResponse {
  id: string;
  title: string;
  content: string | null;
  isPinned: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  backgroundColor: BackgroundColor;
  isDeleted: boolean;
  deletedAt: string | null;
  noteGroupId: string | null;
  createdBy: string;
}

export interface TogglePinResponse {
  id: string;
  isPinned: boolean;
  lastModifiedAt: string;
}

export interface UpdateNoteResponse {
  id: string;
  title: string | null;
  content: string | null;
  lastModifiedAt: string;
  wasUpdated: boolean;
}
