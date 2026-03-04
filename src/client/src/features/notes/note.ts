import { BackgroundColor } from "@/shared/enums/backgroundColor";
import { NoteGroup } from "./noteGroup";
import { User } from "@/shared/types/user";
import { JSONContent } from "@tiptap/react";

/** DTO для получения всех заметок (GetNoteResponse на сервере) */
export interface GetNoteResponse {
  id: string;
  title: string;
  content: JSONContent | null;
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
  content?: JSONContent | null;
  isPinned: boolean;
  backgroundColor: number;
  noteGroupId?: string | null;
}

export interface CreateNoteResponse {
  id: string;
  title: string;
  content: JSONContent | null;
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

export interface UpdateNoteTitleRequest {
  id: string;
  title: string;
}

export interface UpdateNoteContentRequest {
  id: string;
  content: JSONContent;
}

export interface UpdateNoteTitleResponse {
  id: string;
  title: string | null;
  lastModifiedAt: string;
  wasUpdated: boolean;
}

export interface UpdateNoteContentResponse {
  id: string;
  content: JSONContent | null;
  lastModifiedAt: string;
  wasUpdated: boolean;
}
