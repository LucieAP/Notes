/** Пользователь в ответе API (GetUserResponse) */
export interface User {
  id: string;
  name: string;
  email: string;
  picture: string | null;
  emailVerified: boolean;
  lastLoginAt: string;
}
