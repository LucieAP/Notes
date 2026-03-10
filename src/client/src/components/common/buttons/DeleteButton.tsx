import useNotes from "@/shared/hooks/useNotes";
import TrashIcon from "../icons/TrashIcon";

interface Props {
  onDelete?: () => void | Promise<void>;
}

function DeleteButton({ onDelete }: Props) {
  return (
    <button
      onClick={onDelete}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <TrashIcon />
      <span>Delete</span>
    </button>
  );
}

export default DeleteButton;
