import useNotes from "@/shared/hooks/useNotes";
import TrashIcon from "../icons/TrashIcon";

interface Props {
  itemId?: string;
}

function DeleteButton({ itemId }: Props) {
  const { deleteNote } = useNotes();

  return (
    <button
      onClick={() => itemId && deleteNote(itemId)}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <TrashIcon />
      <span>Delete</span>
    </button>
  );
}

export default DeleteButton;
