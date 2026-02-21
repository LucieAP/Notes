import DropdownIcon from "../common/icons/DropdownIcon";
import { NavItem } from "./sidebar.config";
import SidebarNavLink from "./SidebarNavLink";
import { useState } from "react";

interface Props {
  navItems?: NavItem[];
  title: string;
  defaultOpen?: boolean;
}

function SidebarSection({ navItems, title, defaultOpen = true }: Props) {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  return (
    <div className="flex flex-col mt-5">
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex rounded-lg hover:bg-neutral-700 px-1 py-1 space-x-0.5 text-xs cursor-pointer"
      >
        <span>{title}</span>
        <DropdownIcon isOpen={isOpen} />
      </button>

      {isOpen && (
        <div className="flex flex-col">
          {navItems &&
            navItems.map((item) => {
              if (!item) return null;
              return (
                <div key={item.to} className="flex flex-col">
                  <SidebarNavLink
                    key={item.to}
                    to={item.to}
                    icon={<item.icon />}
                    label={item.label}
                    showAddButton={item.showAddButton}
                  />
                </div>
              );
            })}
          {!navItems && (
            <div className="flex items-center text-xs px-2 py-1 text-neutral-500">
              No pages.
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default SidebarSection;
