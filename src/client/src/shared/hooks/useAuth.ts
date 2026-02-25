import { User } from "../types/user";
import { useContext, createContext } from "react";

interface AuthContextType {
  user: User | null;
  loading: boolean;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | null>(null);

function useAuth() {
  const authContext = useContext(AuthContext);

  if (!authContext) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return authContext;
}

export default useAuth;
