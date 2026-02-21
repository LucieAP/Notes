import { NoteGroup } from "./NoteGroup";
import { User } from "./user";

/** Заметка — ответ API (GetNoteResponse) */
export interface Note {
  id: string;
  title: string;
  description: string | null;
  isPinned: boolean;
  createdAt: string;
  lastModifiedAt: string;
  isTrashed: boolean;
  backgroundColor: number;
  isDeleted: boolean;
  deletedAt: string | null;
  noteGroup: NoteGroup | null;
  createdBy: User;
}
