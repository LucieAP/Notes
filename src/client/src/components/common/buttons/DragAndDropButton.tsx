import { ButtonHTMLAttributes, forwardRef } from "react";
import DragAndDropIcon from "../icons/DragAndDroptIcon";

interface Props extends ButtonHTMLAttributes<HTMLButtonElement> {
  isDragging?: boolean;
}

const DragAndDropButton = forwardRef<HTMLButtonElement, Props>(
  ({ isDragging = false, className = "", ...props }, ref) => {
  return (
    <button
      {...props}
      ref={ref}
      className={`flex h-8 w-8 items-center justify-center flex-none touch-none rounded-md 
      border-none outline-none appearance-none bg-transparent select-none text-neutral-400 hover:bg-neutral-700/50 ${
        isDragging ? "cursor-grabbing" : "cursor-grab"
      } ${className}`}
    >
      <DragAndDropIcon />
    </button>
  );
  },
);

export default DragAndDropButton;
