import FavoriteIcon from "../icons/FavoriteIcon";

function AddToFavoritesButton() {
  return (
    <button className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600">
      <FavoriteIcon />
      <span>Add to favorites</span>
    </button>
  );
}

export default AddToFavoritesButton;
