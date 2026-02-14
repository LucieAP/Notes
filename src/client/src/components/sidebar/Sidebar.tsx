import SearchIcon from "../common/icons/SearchIcon";
import SidebarHeader from "./SidebarHeader";
import SidebarNavLink from "./SidebarNavLink";
import { mainNavItems, privateNavItems } from "./sidebar.config";
import SidebarSection from "./SidebarSection";
import SidebarFooter from "./SidebarFooter";

function Sidebar() {
  return (
    <div className="w-60 h-screen border-r border-neutral-800 shrink-0 bg-neutral-800">
      <div className="flex flex-col h-full p-2 gap-0.5">
        <SidebarHeader />
        <div className="flex flex-col px-1 rounded select-none text-sm transition-colors">
          {mainNavItems.map((item) => (
            <SidebarNavLink
              to={item.to}
              icon={<item.icon />}
              label={item.label}
            />
          ))}

          <SidebarNavLink to="/search" icon=<SearchIcon /> label="Search" />
          <SidebarSection navItems={privateNavItems} title="Private" />
        </div>
        <SidebarFooter />
      </div>
    </div>
  );
}

export default Sidebar;
