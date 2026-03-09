import PlusIcon from "../icons/PlusIcon";

interface Props {
  enabled?: boolean;
  onCreate?: () => void;
}

function CreateItemButton({ enabled = true, onCreate }: Props) {
  if (!enabled) return null;

  const handleCreateItem = (e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();
    onCreate?.();
  };

  return (
    <button
      onClick={handleCreateItem}
      aria-label="Create item"
      className="ml-auto rounded-sm hover:bg-neutral-600 p-0.5 shrink-0 cursor-pointer opacity-0 pointer-events-none transition-opacity group-hover:opacity-100 group-hover:pointer-events-auto focus-visible:opacity-100 focus-visible:pointer-events-auto"
    >
      <PlusIcon strokeWidth={3.3} />
    </button>
  );
}

export default CreateItemButton;
