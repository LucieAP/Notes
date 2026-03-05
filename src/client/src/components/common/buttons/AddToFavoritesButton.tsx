import useNotes from "@/shared/hooks/useNotes";
import FavoriteIcon from "../icons/FavoriteIcon";

interface Props {
  itemId?: string;
  onClose?: () => void;
  removeLabel?: string;
  addLabel?: string;
}

function AddToFavoritesButton({
  itemId,
  onClose,
  removeLabel,
  addLabel,
}: Props) {
  if (!itemId) return null;
  const { pinNote, notesData } = useNotes();
  const isPinned = notesData.find((n) => n.id === itemId)?.isPinned ?? false;

  return (
    <button
      onClick={() => {
        pinNote(itemId);
        onClose?.();
      }}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <FavoriteIcon />
      {(addLabel || removeLabel) && (
        <span>{isPinned ? removeLabel : addLabel}</span>
      )}{" "}
    </button>
  );
}

export default AddToFavoritesButton;
