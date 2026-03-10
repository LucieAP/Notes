import { useRef } from "react";
import AddToFavoritesButton from "../buttons/AddToFavoritesButton";
import MoveToTrashButton from "../buttons/MoveToTrashButton";
import DeleteButton from "../buttons/DeleteButton";
import { EntityType } from "@/shared/types/entityType";
import RenameButton from "../buttons/RenameButton";
import useEntity from "@/shared/hooks/useEntity";

interface Props {
  itemId?: string;
  position: { top: number; left: number };
  entityType?: EntityType;
  onClose?: () => void;
  onRename?: () => void;
}

function KebabMenu({ itemId, position, entityType, onClose, onRename }: Props) {
  const backdropRef = useRef<HTMLDivElement>(null);

  if (!entityType || !itemId) return;
  const { toggleFav, deleteItem, trashItem } = useEntity(entityType);

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
        className="flex flex-col fixed rounded-lg p-1 bg-neutral-700 border border-neutral-800 overflow-hidden whitespace-nowrap"
      >
        <AddToFavoritesButton
          itemId={itemId}
          entityType={entityType}
          onToggle={() => {
            toggleFav(itemId);
            onClose?.();
          }}
          removeLabel="Remove from favorites"
          addLabel="Add to favorites"
        />
        <RenameButton
          onRename={() => {
            onRename?.();
            onClose?.();
          }}
        />
        <MoveToTrashButton
          onMoveToTrash={() => {
            trashItem?.(itemId);
            onClose?.();
          }}
        />
        <DeleteButton onDelete={() => deleteItem(itemId)} />
      </div>
    </div>
  );
}

export default KebabMenu;
