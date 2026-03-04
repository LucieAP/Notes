import { GetNoteResponse } from "@/features/notes/note";
import { JSONContent } from "@tiptap/core";
import { createContext, useContext } from "react";

interface NotesContextTypes {
  notesData: GetNoteResponse[];
  fetchNotes: () => Promise<void>;
  createNote: () => Promise<void>;
  getNoteById: (id: string) => Promise<GetNoteResponse | undefined>;
  updateTitle: ({ id, title }: { id: string; title: string }) => Promise<void>;
  updateContent: ({
    id,
    content,
  }: {
    id: string;
    content: JSONContent;
  }) => Promise<void>;
  deleteNote: (id: string) => Promise<void>;
  trashNote: (id: string) => Promise<void>;
  getTrashed: () => Promise<GetNoteResponse[]>;
  restoreNote: (id: string) => Promise<void>;
  pinNote: (id: string) => Promise<void>;
}

export const NotesContext = createContext<NotesContextTypes | null>(null);

function useNotes() {
  const notesContext = useContext(NotesContext);

  if (!notesContext) {
    throw new Error("useNotes must be used within NotesProvider");
  }

  return notesContext;
}

export default useNotes;
