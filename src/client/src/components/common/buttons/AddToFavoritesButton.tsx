import useNotes from "@/shared/hooks/useNotes";
import FavoriteIcon from "../icons/FavoriteIcon";

interface Props {
  itemId?: string;
  onClose?: () => void;
}

function AddToFavoritesButton({ itemId, onClose }: Props) {
  if (!itemId) return;
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

      <span>{isPinned ? "Remove from favorites" : "Add to favorites"}</span>
    </button>
  );
}

export default AddToFavoritesButton;
