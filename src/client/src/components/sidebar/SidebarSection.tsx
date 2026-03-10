import DropdownIcon from "../common/icons/DropdownIcon";
import { NavItem } from "./sidebar.config";
import SidebarNavLink from "./SidebarNavLink";
import { useState } from "react";

import useFavorites from "@/shared/hooks/useFavorites";

interface Props {
  navItems?: NavItem[];
  title: string;
  defaultOpen?: boolean;
  type?: "private" | "favorite";
}

function SidebarSection({ navItems, title, defaultOpen = true, type }: Props) {
  const [isOpen, setIsOpen] = useState(defaultOpen);
  const { favorites, isLoading } = useFavorites();

  return (
    <div className="flex flex-col mt-5">
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex rounded-lg hover:bg-neutral-700 px-1 py-1 space-x-0.5 text-xs cursor-pointer"
      >
        <span>{title}</span>
        <DropdownIcon isOpen={isOpen} />
      </button>

      {isOpen && type === "private" && (
        <div className="flex flex-col">
          {navItems?.map((item) => {
            if (!item) return null;

            return (
              <div key={item.to} className="flex flex-col">
                <SidebarNavLink
                  to={item.to}
                  icon={<item.icon />}
                  label={item.label}
                  onCreate={item.onCreate}
                  entityType={item.entityType}
                />
                {/* Дочерние элементы */}
                {item.children?.map((child) => (
                  <div key={child.to} className="pl-4">
                    <SidebarNavLink
                      itemId={child.itemId}
                      to={child.to}
                      icon={<item.icon />}
                      label={child.label}
                      entityType={item.entityType}
                    />
                  </div>
                ))}
              </div>
            );
          })}
        </div>
      )}

      {isOpen && type === "favorite" && (
        <div className="flex flex-col mt-1">
          {isLoading ? (
            <p className="text-xs text-neutral-500 px-2 py-2">Loading...</p>
          ) : favorites.length === 0 ? (
            <p className="text-xs text-neutral-500 px-2 py-2">
              No favorites yet
            </p>
          ) : (
            <>
              {favorites.map((item) => (
                <SidebarNavLink
                  key={item.id}
                  itemId={item.id}
                  to={`/${item.entityType}/${item.id}`}
                  icon={<item.icon />}
                  label={item.title || "Untitled"}
                  entityType={item.entityType}
                />
              ))}
            </>
          )}
        </div>
      )}
    </div>
  );
}

export default SidebarSection;
