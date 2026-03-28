import api from "@/shared/api/axios";
import { GetTokenResponse } from "@/shared/api/token";
import { User } from "@/shared/types/user";

export const authApi = {
  getUser(): Promise<User> {
    return api.get<User, User>(`/auth/me`);
  },

  refresh(): Promise<GetTokenResponse> {
    return api.post<GetTokenResponse, GetTokenResponse>(`/auth/refresh`);
  },

  logout(): Promise<void> {
    return api.post(`/auth/logout`);
  },
};
