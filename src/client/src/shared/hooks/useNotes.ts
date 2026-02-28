import { GetNoteResponse } from "@/features/notes/note";
import { createContext, useContext } from "react";

interface NotesContextTypes {
  notesData: GetNoteResponse[];
  fetchNotes: () => Promise<void>;
  createNote: () => Promise<void>;
  updateTitle: ({ id, title }: { id: string; title: string }) => Promise<void>;
  // updateContent: () => Promise<void>;
  deleteNote: (id: string) => Promise<void>;
  trashNote: (id: string) => Promise<void>;
  getTrashed: () => Promise<GetNoteResponse[]>;
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
