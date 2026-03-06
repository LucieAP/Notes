import { useCallback, useRef, useState } from "react";

interface Props {
  initialWidth?: number;
  minWidth?: number;
  maxWidth?: number;
}

function useResizableEditorBorder({
  initialWidth = 896,
  minWidth = 672,
  maxWidth = 1280,
}: Props = {}) {
  const [width, setWidth] = useState(() =>
    Math.min(maxWidth, Math.max(minWidth, initialWidth)),
  );

  const startX = useRef(0);
  const startWidth = useRef(width);
  const direction = useRef<"left" | "right">("right");

  const onMouseMove = useCallback(
    (e: MouseEvent) => {
      const delta = e.clientX - startX.current;

      let next =
        direction.current === "right"
          ? startWidth.current + delta
          : startWidth.current - delta;

      next = Math.min(Math.max(next, minWidth), maxWidth);

      setWidth(next);
    },
    [minWidth, maxWidth],
  );

  const onMouseDown = (e: React.MouseEvent, dir: "left" | "right") => {
    direction.current = dir;
    startX.current = e.clientX;
    startWidth.current = width;

    window.addEventListener("mousemove", onMouseMove);
    window.addEventListener("mouseup", onMouseUp);

    document.body.style.userSelect = "none";
    document.body.style.cursor = "col-resize";
  };

  const onMouseUp = useCallback(() => {
    window.removeEventListener("mousemove", onMouseMove);
    window.removeEventListener("mouseup", onMouseUp);

    document.body.style.userSelect = "";
    document.body.style.cursor = "";
  }, [onMouseMove]);

  return [width, setWidth, onMouseDown] as const;
}

export default useResizableEditorBorder;
