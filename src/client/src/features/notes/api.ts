import api from "@/shared/api/axios";
import {
  CreateNoteRequest,
  GetNoteResponse,
  TogglePinResponse,
  UpdateNoteResponse,
} from "./note";

export const notesApi = {
  getAll(): Promise<GetNoteResponse[]> {
    return api.get("/notes");
  },

  getNoteById(id: string): Promise<GetNoteResponse> {
    return api.get(`/notes/${id}`);
  },

  create(data: CreateNoteRequest): Promise<GetNoteResponse> {
    return api.post("/notes/create", data);
  },

  togglePin(id: string): Promise<TogglePinResponse> {
    return api.patch(`notes/${id}/pin`);
  },

  updateTitle({
    id,
    title,
  }: {
    id: string;
    title: string;
  }): Promise<UpdateNoteResponse> {
    return api.patch(`notes/${id}`, title);
  },

  updateContent({
    id,
    content,
  }: {
    id: string;
    content: string;
  }): Promise<UpdateNoteResponse> {
    return api.patch(`notes/${id}`, content);
  },

  delete(id: string) {
    return api.delete(`notes/delete/${id}`);
  },
};
