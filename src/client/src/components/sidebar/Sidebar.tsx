import SidebarHeader from "./SidebarHeader";
import SidebarNavLink from "./SidebarNavLink";
import { mainNavItems, privateNavItems } from "./sidebar.config";
import SidebarSection from "./SidebarSection";
import SidebarFooter from "./SidebarFooter";
import SearchButton from "../common/buttons/SearchButton";
import useResizableSidebar from "@/shared/hooks/useResizableSidebar";
import { useEffect, useState } from "react";

const SIDEBAR_WIDTH_KEY = "sidebar-width";
const DEFAULT_WIDTH = 240;

function Sidebar() {
  const raw = Number(localStorage.getItem(SIDEBAR_WIDTH_KEY));
  const savedWidth = Number.isFinite(raw) ? raw : DEFAULT_WIDTH; // isFinite - если число и не NaN

  const [width, setWidth, onMouseDown] = useResizableSidebar({
    initialWidth: savedWidth,
  });

  useEffect(() => {
    localStorage.setItem(SIDEBAR_WIDTH_KEY, width.toString());
  }, [width]);

  function handleAddPage() {}

  return (
    <div
      style={{ width }}
      className="relative h-screen border-r border-neutral-800 shrink-0 bg-neutral-800"
    >
      {/* Content */}
      <div className="flex flex-1 flex-col h-full p-2 gap-0.5 overflow-hidden">
        <SidebarHeader />
        <div className="flex flex-col px-1 rounded select-none text-sm transition-colors">
          {mainNavItems.map((item) => (
            <SidebarNavLink
              key={item.to}
              to={item.to}
              icon={<item.icon />}
              label={item.label}
              showAddButton={item.showAddButton}
              onAdd={handleAddPage}
            />
          ))}

          <SearchButton />

          <SidebarSection navItems={privateNavItems} title="Private" />
          <SidebarSection title="Favorite" />
        </div>
        <SidebarFooter />
      </div>

      {/* Resize Handle */}
      <div
        onMouseDown={onMouseDown}
        onDoubleClick={() => setWidth(DEFAULT_WIDTH)}
        className="absolute top-0 right-0 h-full w-1 cursor-col-resize hover:bg-neutral-700 active:bg-neutral-600"
      />
    </div>
  );
}

export default Sidebar;
