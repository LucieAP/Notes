import { useEffect, useRef, useState } from "react";
import SearchIcon from "../icons/SearchIcon";
import useNotes from "@/shared/hooks/useNotes";
import { GetNoteResponse } from "@/features/notes/note";
import useTrashed, { TrashedItem } from "@/shared/hooks/useTrashed";

interface Props {
  position?: { bottom: number; left: number };
  onClose?: () => void;
}

function TrashMenu({ position, onClose }: Props) {
  const backdropRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const [trashed, setTrashed] = useState<TrashedItem[]>([]);

  const { fetchTrashedItems, deleteItem, restoreItem } = useTrashed();

  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadTrashedItems = async () => {
      try {
        const data = await fetchTrashedItems();
        setTrashed(data);
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      } finally {
        setIsLoading(false);
      }
    };

    loadTrashedItems();
  }, []);

  const handleDelete = async (item: TrashedItem) => {
    await deleteItem(item.id, item.entityType);
    setTrashed((prev) => prev.filter((n) => n.id !== item.id));
  };

  const handleRestore = async (item: TrashedItem) => {
    await restoreItem(item.id, item.entityType);
    setTrashed((prev) => prev.filter((n) => n.id !== item.id));
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
        <div className="flex flex-col max-h-64 min-h-40 overflow-y-auto">
          {isLoading ? (
            <div className="text-sm text-neutral-500 text-center py-4">
              Loading...
            </div>
          ) : trashed.length === 0 ? (
            <div className="text-sm text-neutral-500 text-center py-4">
              No pages in Trash
            </div>
          ) : (
            trashed.map((item) => (
              <div
                key={item.id}
                className="group flex items-center justify-between px-3 py-2 hover:bg-neutral-800 text-sm"
              >
                <span className="truncate text-neutral-200">
                  {item.title || "Untitled"}
                </span>
                <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity shrink-0 ml-2">
                  <button
                    onClick={() => handleDelete(item)}
                    className="text-xs text-red-400 hover:text-red-300 px-1.5 py-0.5 rounded hover:bg-neutral-700"
                  >
                    Delete
                  </button>
                  <button
                    onClick={() => handleRestore(item)}
                    className="text-xs text-green-400 hover:text-green-300 px-1.5 py-0.5 rounded hover:bg-neutral-700"
                  >
                    Restore
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
