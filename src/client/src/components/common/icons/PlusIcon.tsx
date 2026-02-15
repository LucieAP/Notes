import cn from "@/shared/utils/cn";

interface PlusIconProps {
  isOpen?: boolean;
  className?: string;
  strokeWidth?: number;
}

function PlusIcon({
  isOpen = true,
  className,
  strokeWidth = 2,
}: PlusIconProps) {
  return (
    <svg
      className={cn("my-auto size-3.5", className)}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={strokeWidth}
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M12 5v14M5 12h14" />
    </svg>
  );
}

export default PlusIcon;
