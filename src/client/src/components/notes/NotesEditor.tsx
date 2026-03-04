import { useParams } from "react-router-dom";

function NotesEditorPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <div>
      <h1>notes/{id ?? ""} </h1>
    </div>
  );
}

export default NotesEditorPage;
