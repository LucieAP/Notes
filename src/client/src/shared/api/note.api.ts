import api from "./axios";
import type { Note } from "../types/note";

export const notesApi = {
  getAll(): Promise<Note[]> {
    return api.get("/notes");
  },
};
