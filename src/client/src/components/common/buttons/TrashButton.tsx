import TrashIcon from "../icons/TrashIcon";

function TrashButton() {
  return (
    <button
      type="button"
      className="flex items-center gap-2 px-2 py-1 rounded-lg hover:bg-neutral-700 transition-colors select-none w-full cursor-pointer"
    >
      <TrashIcon />
      <span className="text-sm">Trash</span>
    </button>
  );
}

export default TrashButton;
