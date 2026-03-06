import AddToFavoritesButton from "@/components/common/buttons/AddToFavoritesButton";
import { useParams } from "react-router-dom";

interface EditorHeaderProps {
  title?: string;
}

function EditorHeader({ title }: EditorHeaderProps) {
  const { id } = useParams();

  return (
    <div className="sticky top-0 z-10 flex h-7 mx-5 my-1.5 items-center justify-between bg-background">
      {title ? <div className="flex">{title}</div> : <div>Undefined</div>}

      <AddToFavoritesButton itemId={id} />
    </div>
  );
}

export default EditorHeader;
