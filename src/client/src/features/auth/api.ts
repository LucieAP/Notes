import api from "@/shared/api/axios";
import type { AxiosRequestConfig } from "axios";
import { GetTokenResponse } from "@/shared/api/token";
import { User } from "@/shared/types/user";

export const authApi = {
  getUser(config?: AxiosRequestConfig): Promise<User> {
    return api.get<User, User>(`/auth/me`, config);
  },

  refresh(config?: AxiosRequestConfig): Promise<GetTokenResponse> {
    return api.post<GetTokenResponse, GetTokenResponse>(
      `/auth/refresh`,
      undefined,
      config,
    );
  },

  logout(config?: AxiosRequestConfig): Promise<void> {
    return api.post(`/auth/logout`, undefined, config);
  },
};
