import { notesApi } from "@/features/notes/api";
import { GetNoteResponse } from "@/features/notes/note";
import { NotesContext } from "@/shared/hooks/useNotes";
import { useCallback, useEffect, useMemo, useState } from "react";

function NotesProvider({ children }: { children: React.ReactNode }) {
  const [notesData, setNotesData] = useState<GetNoteResponse[]>([]);

  const fetchNotes = useCallback(async () => {
    try {
      const data = await notesApi.getAll();
      setNotesData(data ?? []);
      console.log("fetched:", data);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  useEffect(() => {
    fetchNotes();
  }, []);

  const createNote = useCallback(async () => {
    try {
      const createdNote = await notesApi.create({
        title: "New Note",
        isPinned: false,
        backgroundColor: 0,
      });

      setNotesData((prev) => [...prev, createdNote]);
      console.log(createdNote);
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  //   const getNoteById = async () => {
  //     try {
  //       if (!params.id) return;

  //       const note = await notesApi.getNoteById(params.id);
  //       console.log(note);
  //     } catch (err) {
  //       console.log(err instanceof Error ? err.message : err);
  //     }
  //   };

  const deleteNote = useCallback(
    async (id: string) => {
      try {
        if (!id) return;

        await notesApi.delete(id);
        console.log(`Deleted note: ${id} `);
        fetchNotes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchNotes],
  );

  const value = useMemo(() => {
    return { notesData, fetchNotes, createNote, deleteNote };
  }, [notesData, fetchNotes, createNote, deleteNote]);

  return <NotesContext value={value}> {children}</NotesContext>;
}

export default NotesProvider;
