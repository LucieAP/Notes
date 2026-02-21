import PlusIcon from "../icons/PlusIcon";

interface Props {
  enabled?: boolean;
  onAdd?: () => void;
}

function AddItemButton({ enabled = true, onAdd }: Props) {
  if (!enabled) return null;

  const handleAddItem = (e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();
    onAdd?.();
  };

  return (
    <button
      onClick={handleAddItem}
      aria-label="Add item"
      className="ml-auto rounded-sm hover:bg-neutral-600 p-0.5 shrink-0 cursor-pointer opacity-0 pointer-events-none transition-opacity group-hover:opacity-100 group-hover:pointer-events-auto focus-visible:opacity-100 focus-visible:pointer-events-auto"
    >
      <PlusIcon className="" strokeWidth={3.3} />
    </button>
  );
}

export default AddItemButton;
