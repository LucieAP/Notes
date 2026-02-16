import SidebarHeader from "./SidebarHeader";
import SidebarNavLink from "./SidebarNavLink";
import { mainNavItems, privateNavItems } from "./sidebar.config";
import SidebarSection from "./SidebarSection";
import SidebarFooter from "./SidebarFooter";
import SearchButton from "../common/buttons/SearchButton";
import { MouseEvent, useState } from "react";

function Sidebar() {
  // function handleClick(e: MouseEvent<HTMLButtonElement>) {
  //   e.preventDefault();

  //   console.log("SearchButton Clicked!");
  // }

  return (
    <div className="w-60 h-screen border-r border-neutral-800 shrink-0 bg-neutral-800">
      <div className="flex flex-col h-full p-2 gap-0.5">
        <SidebarHeader />
        <div className="flex flex-col px-1 rounded select-none text-sm transition-colors">
          {mainNavItems.map((item) => (
            <SidebarNavLink
              key={item.to}
              to={item.to}
              icon={<item.icon />}
              label={item.label}
              showAddButton={item.showAddButton}
            />
          ))}

          <SearchButton />

          <SidebarSection navItems={privateNavItems} title="Private" />
          <SidebarSection title="Favorite" />
        </div>
        <SidebarFooter />
      </div>
    </div>
  );
}

export default Sidebar;
