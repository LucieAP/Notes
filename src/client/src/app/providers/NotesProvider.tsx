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

  const updateTitle = useCallback(
    async ({ id, title }: { id: string; title: string }) => {
      try {
        if (!id) return;
        await notesApi.updateTitle({ id, title });
        fetchNotes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchNotes],
  );

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

  const trashNote = useCallback(
    async (id: string) => {
      try {
        if (!id) return;

        await notesApi.trash(id);
        fetchNotes();
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    },
    [fetchNotes],
  );

  const getTrashed = useCallback(async () => {
    try {
      const data = await notesApi.getTrashed();
      return data ?? [];
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
      throw err;
    }
  }, []);

  const pinNote = useCallback(async (id: string) => {
    try {
      const data = await notesApi.togglePin(id);
      setNotesData((prev) =>
        prev.map((note) =>
          note.id === id ? { ...note, isPinned: data.isPinned } : note,
        ),
      );
    } catch (err) {
      console.log(err instanceof Error ? err.message : err);
    }
  }, []);

  const value = useMemo(() => {
    return {
      notesData,
      fetchNotes,
      createNote,
      updateTitle,
      deleteNote,
      trashNote,
      getTrashed,
      pinNote,
    };
  }, [
    notesData,
    fetchNotes,
    createNote,
    updateTitle,
    deleteNote,
    trashNote,
    getTrashed,
    pinNote,
  ]);

  return <NotesContext value={value}> {children}</NotesContext>;
}

export default NotesProvider;
