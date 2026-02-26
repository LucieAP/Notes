import RenameIcon from "../icons/RenameIcon";

function RenameButton() {
  return (
    <button className="flex space-x-1 rounded-lg p-1 cursor-pointer hover:bg-neutral-600">
      <RenameIcon />
      <span>Rename</span>
    </button>
  );
}

export default RenameButton;
