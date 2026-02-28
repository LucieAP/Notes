import { useRef } from "react";
import AddToFavoritesButton from "../buttons/AddToFavoritesButton";
import RenameNoteButton from "../buttons/RenameNoteButton";
import MoveToTrashButton from "../buttons/MoveToTrashButton";
import DeleteButton from "../buttons/DeleteButton";

interface Props {
  itemId?: string;
  position: { top: number; left: number };
  onClose?: () => void;
  onRename?: () => void;
}

function KebabMenu({ itemId, position, onClose, onRename }: Props) {
  const backdropRef = useRef<HTMLDivElement>(null);

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
        style={{ top: position.top, left: position.left }}
        onClick={(e) => e.stopPropagation()}
        className="flex flex-col fixed rounded-lg p-1 bg-neutral-700 border border-neutral-800 overflow-hidden"
      >
        <AddToFavoritesButton />
        <RenameNoteButton onRename={onRename} onClose={onClose} />
        <MoveToTrashButton itemId={itemId} onClose={onClose} />
        <DeleteButton itemId={itemId} />
      </div>
    </div>
  );
}

export default KebabMenu;
