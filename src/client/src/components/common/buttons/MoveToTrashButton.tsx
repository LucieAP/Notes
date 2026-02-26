import TrashIcon from "../icons/TrashIcon";

function MoveToTrashButton() {
  return (
    <button className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600">
      <TrashIcon />
      <span>Move to Trash</span>
    </button>
  );
}

export default MoveToTrashButton;
