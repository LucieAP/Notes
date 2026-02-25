import { GetNoteResponse } from "@/features/notes/note";
import NoteIcon from "../common/icons/NoteIcon";
import type { MouseEvent } from "react";

interface Props {
  note: GetNoteResponse;
}

function NoteCard({ note }: Props) {
  return (
    <div className="flex space-x-1 p-2 hover:bg-neutral-600 rounded-lg">
      <NoteIcon />
      <span className="select-none underline decoration-neutral-600 underline-offset-3">
        {note.title}
      </span>
    </div>
  );
}

export default NoteCard;
