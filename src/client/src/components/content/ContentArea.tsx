import { useEffect, useState } from "react";
import { Outlet, useLocation, useMatch } from "react-router-dom";
import api from "@/shared/api/axios";
import { notesApi } from "@/shared/api/note.api";
import { Note } from "@/shared/types/note";
import NoteCard from "../notes/NoteCard";

function ContentArea() {
  const [notesData, setNotesData] = useState<Note[]>([]);

  // const match = useMatch();
  const location = useLocation();

  useEffect(() => {
    async function fetchNotes() {
      try {
        const data = await notesApi.getAll();
        setNotesData(data ?? []);
        console.log("fetched:", data);
      } catch (err) {
        console.log(err instanceof Error ? err.message : err);
      }
    }
    fetchNotes();
  }, []);

  return (
    <>
      <Outlet />
      {location.pathname === "/notes" && (
        <div className="flex overflow-y-auto w-full justify-center">
          <div className="flex flex-col px-24 py-20 max-w-md w-full justify-center">
            <span className="text-2xl pl-2 font-bold">Notes</span>
            {notesData.map((note) => {
              return <NoteCard key={note.id} note={note} />;
            })}
          </div>
        </div>
      )}
    </>
  );
}

export default ContentArea;
