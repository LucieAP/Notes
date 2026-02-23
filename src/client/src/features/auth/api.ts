import api from "@/shared/api/axios";
import { User } from "@/shared/types/user";

export const authApi = {
  getUser(): Promise<User> {
    return api.get<User, User>("/auth/me");
  },
};
