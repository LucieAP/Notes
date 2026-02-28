import { useRef, useState } from "react";
import TrashIcon from "../icons/TrashIcon";
import { createPortal } from "react-dom";
import TrashMenu from "../menus/TrashMenu";

function TrashButton() {
  const [isOpen, setIsOpen] = useState(false);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const [position, setPosition] = useState({ bottom: 0, left: 0 });

  const handleOpen = (e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();

    const rect = buttonRef.current?.getBoundingClientRect();

    if (rect) {
      // console.log("rect ", rect);

      const position = {
        bottom: window.innerHeight - rect.top,
        left: rect.right,
      };
      setPosition(position);
    }

    setIsOpen(true);
  };

  return (
    <>
      <button
        ref={buttonRef}
        type="button"
        onClick={handleOpen}
        className="flex items-center gap-2 px-2 py-1 rounded-lg hover:bg-neutral-700 transition-colors select-none w-full cursor-pointer"
      >
        <TrashIcon />
        <span className="text-sm">Trash</span>
      </button>

      {isOpen &&
        createPortal(
          <TrashMenu position={position} onClose={() => setIsOpen(false)} />,
          document.body,
        )}
    </>
  );
}

export default TrashButton;
