import { useEffect, useRef, useState } from "react";
import SearchIcon from "../icons/SearchIcon";
import useNotes from "@/shared/hooks/useNotes";
import { GetNoteResponse } from "@/features/notes/note";
import { notesApi } from "@/features/notes/api";

interface Props {
  position?: { bottom: number; left: number };
  onClose?: () => void;
}

function TrashMenu({ position, onClose }: Props) {
  const backdropRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const [trashedNotes, setTrashed] = useState<GetNoteResponse[]>([]);

  const { deleteNote, getTrashed } = useNotes();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchTrashed = async () => {
      const trashedNotes = await getTrashed();
      setTrashed(trashedNotes);
      setIsLoading(false);
    };
    fetchTrashed();
  }, []);

  const handleDelete = async (id: string) => {
    await deleteNote(id);
    setTrashed((prev) => prev.filter((n) => n.id !== id));
  };

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        onClose?.();
      }
    };

    window.addEventListener("keydown", handler);

    return () => window.removeEventListener("keydown", handler);
  }, []);

  return (
    <div
      ref={backdropRef}
      onMouseDown={(e) => {
        if (e.target === backdropRef.current) {
          onClose?.();
        }
      }}
      className="fixed inset-0 z-50"
    >
      <div
        style={{ bottom: position?.bottom, left: position?.left }}
        onClick={(e) => e.stopPropagation()}
        className="flex flex-col fixed space-x-1 rounded-lg max-w-85 bg-neutral-900 overflow-hidden"
      >
        {/* Header */}
        <div className="flex p-2">
          <div className="flex items-center border pl-4 gap-2 border-gray-500/30 h-8 rounded-full overflow-hidden w-full">
            <SearchIcon type={2} />
            <input
              type="text"
              ref={inputRef}
              placeholder="Search"
              className="w-full h-full outline-none bg-transparent placeholder-gray-500 text-sm select-none"
            />
          </div>
        </div>
        <div className="flex flex-col max-h-64 overflow-y-auto">
          {isLoading ? (
            <div className="text-sm text-neutral-500 text-center py-4">
              Loading...
            </div>
          ) : trashedNotes.length === 0 ? (
            <div className="text-sm text-neutral-500 text-center py-4">
              No pages in Trash
            </div>
          ) : (
            trashedNotes.map((note) => (
              <div
                key={note.id}
                className="group flex items-center justify-between px-3 py-2 hover:bg-neutral-800 text-sm"
              >
                <span className="truncate text-neutral-200">
                  {note.title || "Untitled"}
                </span>
                <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity shrink-0 ml-2">
                  <button
                    onClick={() => handleDelete(note.id)}
                    className="text-xs text-red-400 hover:text-red-300 px-1.5 py-0.5 rounded hover:bg-neutral-700"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))
          )}
        </div>
        {/* Footer */}
        <div className="flex text-xs px-2 p-2 text-neutral-400 pt-3 bg-neutral-700">
          Pages in Trash for over 30 days will be automatically deleted
        </div>
      </div>
    </div>
  );
}

export default TrashMenu;
