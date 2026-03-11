import { GetNoteResponse } from "@/features/notes/note";
import { useCallback, useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import { SimpleEditor } from "../tiptap/templates/simple/SimpleEditor";
import useNotes from "@/shared/hooks/useNotes";
import { JSONContent } from "@tiptap/core";
import Spinner from "../common/icons/Spinner";

function NotesEditorPage() {
  const { id } = useParams<{ id: string }>();
  const [note, setNote] = useState<GetNoteResponse | null>(null);
  const saveTimeoutRef = useRef<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const { getNoteById, updateContent } = useNotes();

  useEffect(() => {
    if (!id) return;

    (async () => {
      const fetchedNote = await getNoteById(id);
      if (fetchedNote) {
        setNote(fetchedNote);
        console.log("note: ", fetchedNote);
      }
      setIsLoading(false);
    })();
  }, [id, getNoteById]);

  const handleContentChange = useCallback(
    (content: JSONContent) => {
      if (!id) return;

      // очистка предыдущего таймера
      if (saveTimeoutRef.current) {
        window.clearTimeout(saveTimeoutRef.current);
      }

      // создание нового таймера
      saveTimeoutRef.current = window.setTimeout(async () => {
        try {
          await updateContent({ id, content });
        } catch (err) {
          console.log(err instanceof Error ? err.message : err);
        }
      }, 1000); // 1 секунда паузы после ввода
    },
    [id, updateContent],
  );

  return (
    <>
      {!isLoading ? (
        note ? (
          <SimpleEditor
            key={note.id}
            initialContent={note?.content}
            noteTitle={note?.title}
            onChange={handleContentChange}
          />
        ) : (
          <div>Note not found</div>
        )
      ) : (
        <Spinner />
      )}
    </>
  );
}

export default NotesEditorPage;
