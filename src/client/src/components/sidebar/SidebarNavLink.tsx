import cn from "@/shared/utils/cn";
import { NavLink, useMatch } from "react-router-dom";
import CreateItemButton from "../common/buttons/CreateItemButton";
import useNotes from "@/shared/hooks/useNotes";
import KebabButton from "../common/buttons/KebabButton";
import { useRef, useState } from "react";
import { notesApi } from "@/features/notes/api";

interface Props {
  itemId?: string;
  to: string;
  icon: React.ReactNode;
  label: string;
  showAddButton?: boolean;
}

function SidebarNavLink({
  itemId,
  to,
  icon,
  label,
  showAddButton = true,
}: Props) {
  const match = useMatch(to); // совпадает ли текущий URL с заданным маршрутом
  const isActive = Boolean(match);

  const { createNote } = useNotes();

  const [value, setValue] = useState(label);
  const [isEditing, setIsEditing] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const { updateTitle } = useNotes();

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
      <div className="flex flex-1 items-center space-x-2">
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
          <NavLink to={to} className="flex flex-1 items-center space-x-2">
            <span>{label}</span>
          </NavLink>
        )}
      </div>

      {itemId && <KebabButton itemId={itemId} onRename={handleRenameStart} />}
      {showAddButton && <CreateItemButton onCreateNote={createNote} />}
    </div>
  );
}

export default SidebarNavLink;
