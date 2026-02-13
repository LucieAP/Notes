import TrashIcon from "./icons/TrashIcon";

function Trash() {
  return (
    <div className="flex items-center gap-2 px-2 py-1 rounded-lg hover:bg-neutral-700 cursor-pointer transition-colors select-none">
      <TrashIcon />
      <span className="text-sm">Trash</span>
    </div>
  );
}

export default Trash;
