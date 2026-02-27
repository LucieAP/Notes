import { createPortal } from "react-dom";
import KebabIcon from "../icons/KebabIcon";
import { useRef, useState } from "react";
import KebabMenu from "../menus/KebabMenu";

interface Props {
  itemId?: string;
  onRename?: () => void;
}

function KebabButton({ itemId, onRename }: Props) {
  const [isOpen, setIsOpen] = useState(false);
  const [position, setPosition] = useState({ top: 0, left: 0 });
  const buttonRef = useRef<HTMLButtonElement>(null);

  const handleOpen = (e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();

    const rect = buttonRef.current?.getBoundingClientRect();

    if (rect) {
      const position = { top: rect.bottom + 4, left: rect.left };
      setPosition(position);
    }

    setIsOpen(true);
  };
  return (
    <>
      <button
        ref={buttonRef}
        onClick={handleOpen}
        aria-label="Kebab button"
        className="ml-auto rounded-sm hover:bg-neutral-600 p-0.5 shrink-0 cursor-pointer opacity-0 pointer-events-none transition-opacity group-hover:opacity-100 group-hover:pointer-events-auto focus-visible:opacity-100 focus-visible:pointer-events-auto"
      >
        <KebabIcon />
      </button>
      {isOpen &&
        createPortal(
          <KebabMenu
            itemId={itemId}
            position={position}
            onClose={() => setIsOpen(false)}
            onRename={onRename}
          />,
          document.body,
        )}
    </>
  );
}

export default KebabButton;
