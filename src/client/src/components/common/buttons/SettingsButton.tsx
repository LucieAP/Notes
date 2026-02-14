import SettingsIcon from "../icons/SettingsIcon";

function SettingsButton() {
  return (
    <button
      type="button"
      className="flex items-center gap-2 px-2 py-1 rounded-lg hover:bg-neutral-700 transition-colors select-none w-full cursor-pointer"
    >
      <SettingsIcon />
      <span className="text-sm">Settings</span>
    </button>
  );
}

export default SettingsButton;
