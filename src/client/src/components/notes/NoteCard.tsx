import { Note } from "@/shared/types/note";
import { NavLink } from "react-router-dom";
import NoteIcon from "../common/icons/NoteIcon";

function NoteCard({ note }: { note: Note }) {
  return (
    <div className="flex space-x-1 p-2 hover:bg-neutral-600 cursor-pointer rounded-lg">
      <NoteIcon />
      <NavLink
        to={"/notes"}
        className="select-none underline decoration-neutral-600 underline-offset-3"
      >
        {note.title}
      </NavLink>
    </div>
  );
}

export default NoteCard;
