import DropdownIcon from "../common/icons/DropdownIcon";
import { NavItem } from "./sidebar.config";
import SidebarNavLink from "./SidebarNavLink";
import { useState } from "react";

interface Props {
  navItems: NavItem[];
  title: string;
  defaultOpen?: boolean;
}

function SidebarSection({ navItems, title, defaultOpen = true }: Props) {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  return (
    <div className="flex flex-col mt-5">
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex rounded-lg hover:bg-neutral-700 px-1 py-1 space-x-2 text-xs cursor-pointer"
      >
        <span>{title}</span>
        <DropdownIcon isOpen={isOpen} />
      </button>

      {isOpen && (
        <>
          {navItems.map((item) => (
            <SidebarNavLink
              to={item.to}
              icon={<item.icon />}
              label={item.label}
            />
          ))}
        </>
      )}
    </div>
  );
}

export default SidebarSection;
