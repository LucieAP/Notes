import FavoriteIcon from "../icons/FavoriteIcon";
import useEntity from "@/shared/hooks/useEntity";
import { EntityType } from "@/shared/types/entityType";

interface Props {
  itemId?: string;
  entityType?: EntityType;
  onToggle?: () => void;
  removeLabel?: string;
  addLabel?: string;
}

function AddToFavoritesButton({
  itemId,
  entityType,
  onToggle,
  removeLabel,
  addLabel,
}: Props) {
  if (!entityType || !itemId) return;

  const { data, isFavorite } = useEntity(entityType);
  const item = data.find((i) => i.id === itemId);
  const favorited = item ? isFavorite(item) : false;

  return (
    <button
      onClick={onToggle}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <FavoriteIcon />
      {(addLabel || removeLabel) && (
        <span>{favorited ? removeLabel : addLabel}</span>
      )}
    </button>
  );
}

export default AddToFavoritesButton;
