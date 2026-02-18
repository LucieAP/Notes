import { useCallback, useRef, useState } from "react";

interface Props {
  initialWidth?: number;
  minWidth?: number;
  maxWidth?: number;
}

function useResizableSidebar({
  initialWidth = 240,
  minWidth = 180,
  maxWidth = 420,
}: Props = {}) {
  const [width, setWidth] = useState(
    () => Math.min(maxWidth, Math.max(minWidth, initialWidth)), // Значение из localStorage должно быть в пределах
  );
  const isDragging = useRef(false);

  const onMouseMove = useCallback(
    (e: MouseEvent) => {
      if (!isDragging.current) return;

      setWidth((prev) => {
        const next = prev + e.movementX;
        return Math.min(Math.max(next, minWidth), maxWidth);
      });
    },
    [minWidth, maxWidth],
  );

  const onMouseDown = () => {
    isDragging.current = true;
    window.addEventListener("mousemove", onMouseMove);
    window.addEventListener("mouseup", onMouseUp);

    document.body.style.userSelect = "none";
    document.body.style.cursor = "col-resize";
  };

  const onMouseUp = useCallback(() => {
    isDragging.current = false;
    window.removeEventListener("mousemove", onMouseMove);
    window.removeEventListener("mouseup", onMouseUp);

    document.body.style.userSelect = "";
    document.body.style.cursor = "";
  }, [onMouseMove]);

  return [width, setWidth, onMouseDown] as const;
}

export default useResizableSidebar;
