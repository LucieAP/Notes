import useNotes from "@/shared/hooks/useNotes";
import TrashIcon from "../icons/TrashIcon";

interface Props {
  itemId?: string;
  onClose?: () => void;
}

function MoveToTrashButton({ itemId, onClose }: Props) {
  if (!itemId || !onClose) return;

  const { trashNote } = useNotes();

  return (
    <button
      onClick={() => {
        trashNote?.(itemId);
        onClose?.();
      }}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <TrashIcon />
      <span>Move to Trash</span>
    </button>
  );
}

export default MoveToTrashButton;
