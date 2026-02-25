import { NavLink, Outlet, useMatch } from "react-router-dom";
import NoteCard from "../notes/NoteCard";
import useNotes from "@/shared/hooks/useNotes";

function ContentArea() {
  const matchNotes = useMatch("/notes");
  const { notesData } = useNotes();

  return (
    <>
      <Outlet />
      {matchNotes && (
        <div className="flex overflow-y-auto w-full justify-center">
          <div className="flex flex-col px-24 py-20 max-w-md w-full justify-center">
            <span className="text-2xl pl-2 font-bold">Notes</span>
            {notesData.map((note) => {
              return (
                <NavLink key={note.id} to={`/notes/${note.id}`}>
                  <NoteCard note={note} />
                </NavLink>
              );
            })}
          </div>
        </div>
      )}
    </>
  );
}

export default ContentArea;
