import RenameIcon from "../icons/RenameIcon";

interface Props {
  onRename?: () => void;
}

function RenameButton({ onRename }: Props) {
  return (
    <button
      onClick={onRename}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <RenameIcon />
      <span>Rename</span>
    </button>
  );
}

export default RenameButton;
