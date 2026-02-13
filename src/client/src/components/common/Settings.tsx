import SettingsIcon from "./icons/SettingsIcon";

function Settings() {
  return (
    <div className="flex items-center gap-2 px-2 py-1 rounded-lg hover:bg-neutral-700 cursor-pointer transition-colors select-none">
      <SettingsIcon />
      <span className="text-sm">Settings</span>
    </div>
  );
}

export default Settings;
