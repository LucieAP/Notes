import SettingsButton from "../common/buttons/SettingsButton";
import TrashButton from "../common/buttons/TrashButton";

function SidebarFooter() {
  return (
    <div className="flex flex-col px-1 rounded select-none text-sm transition-colors mt-auto mb-20">
      <SettingsButton />
      <TrashButton />
    </div>
  );
}

export default SidebarFooter;
