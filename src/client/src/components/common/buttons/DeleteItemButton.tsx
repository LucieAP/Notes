import TrashIcon from "../icons/TrashIcon";

interface Props {
  onDeleteNote: () => Promise<void>;
}

function DeleteItemButton({ onDeleteNote }: Props) {
  const handleDeleteItem = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    onDeleteNote?.();
  };
  return (
    <button
      onClick={handleDeleteItem}
      aria-label="Delete item"
      className="ml-auto rounded-sm hover:bg-neutral-600 p-0.5 shrink-0 cursor-pointer opacity-0 pointer-events-none transition-opacity group-hover:opacity-100 group-hover:pointer-events-auto focus-visible:opacity-100 focus-visible:pointer-events-auto"
    >
      <TrashIcon />
    </button>
  );
}

export default DeleteItemButton;
