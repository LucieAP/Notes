import RenameIcon from "../icons/RenameIcon";

interface Props {
  onRename?: () => void;
  onClose?: () => void;
}

function RenameNoteButton({ onRename, onClose }: Props) {
  if (!onRename) return;

  return (
    <button
      onClick={() => {
        onRename?.();
        onClose?.();
      }}
      className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600"
    >
      <RenameIcon />
      <span>Rename</span>
    </button>
  );
}

export default RenameNoteButton;
