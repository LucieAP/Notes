import cn from "@/shared/utils/cn";
import { NavLink, useMatch } from "react-router-dom";
import CreateItemButton from "../common/buttons/CreateItemButton";
import KebabButton from "../common/buttons/KebabButton";
import { useRef, useState } from "react";
import { EntityType } from "@/shared/types/entityType";
import useEntity from "@/shared/hooks/useEntity";

interface Props {
  itemId?: string;
  to: string;
  icon: React.ReactNode;
  label: string;
  entityType?: EntityType;
  onCreate?: () => Promise<void>;
}

function SidebarNavLink({
  itemId,
  to,
  icon,
  label,
  entityType,
  onCreate,
}: Props) {
  const match = useMatch(to); // совпадает ли текущий URL с заданным маршрутом
  const isActive = Boolean(match);

  const [value, setValue] = useState(label);
  const [isEditing, setIsEditing] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  if (!entityType) return;

  const { updateTitle } = useEntity(entityType);

  const handleRenameStart = () => {
    setIsEditing(true);
    // выделение текста после рендера
    setTimeout(() => inputRef.current?.select(), 0);
  };

  const handleRenameSubmit = async () => {
    const trimmed = value.trim();
    if (trimmed && trimmed !== label && itemId) {
      await updateTitle({ id: itemId, title: trimmed });
    } else {
      setValue(label); // откат
    }

    setIsEditing(false);
  };

  return (
    <div
      className={cn(
        "group flex items-center rounded-lg px-2 py-1",
        isActive ? "bg-neutral-700" : "hover:bg-neutral-700",
      )}
    >
      <div className="flex flex-1 items-center space-x-2 min-w-0">
        {icon}
        {isEditing ? (
          <input
            ref={inputRef}
            type="text"
            spellCheck={false}
            onBlur={handleRenameSubmit}
            onKeyDown={(e) => {
              if (e.key === "Enter") handleRenameSubmit();
              if (e.key === "Escape") {
                setValue(label);
                setIsEditing(false);
              }
            }}
            value={value}
            onChange={(e) => setValue(e.target.value)}
            className="bg-neutral-600 text-white text-sm rounded px-1 outline-none w-full"
            autoFocus
          />
        ) : (
          <NavLink
            to={to}
            className="flex flex-1 items-center space-x-2 min-w-0"
          >
            <span className="truncate">{label}</span>
          </NavLink>
        )}
      </div>

      {itemId && (
        <KebabButton
          itemId={itemId}
          entityType={entityType}
          onRename={handleRenameStart}
        />
      )}
      {onCreate && <CreateItemButton onCreate={onCreate} />}
    </div>
  );
}

export default SidebarNavLink;
