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
  const [width, setWidth] = useState(() =>
    Math.min(maxWidth, Math.max(minWidth, initialWidth)),
  );

  const isDragging = useRef(false);
  const startX = useRef(0); // начальная координата X при захвате
  const startWidth = useRef(width); // начальная ширина при захвате

  // обработчик движения мыши
  const onMouseMove = useCallback(
    (e: MouseEvent) => {
      if (!isDragging.current) return;

      const deltaX = e.clientX - startX.current; // разница в позиции мыши с начальной координатой
      const nextWidth = Math.min(
        Math.max(startWidth.current + deltaX, minWidth),
        maxWidth,
      );

      setWidth(nextWidth);
    },
    [minWidth, maxWidth],
  );

  // обработчик отпускания мыши
  const onMouseUp = useCallback(() => {
    isDragging.current = false;
    window.removeEventListener("mousemove", onMouseMove);
    window.removeEventListener("mouseup", onMouseUp);

    document.body.style.userSelect = "";
    document.body.style.cursor = "";
  }, [onMouseMove]);

  // обработчик нажатия мыши
  const onMouseDown = (e: React.MouseEvent) => {
    isDragging.current = true;
    startX.current = e.clientX;
    startWidth.current = width;

    window.addEventListener("mousemove", onMouseMove);
    window.addEventListener("mouseup", onMouseUp);

    document.body.style.userSelect = "none";
    document.body.style.cursor = "col-resize";
  };

  return [width, setWidth, onMouseDown] as const;
}

export default useResizableSidebar;
