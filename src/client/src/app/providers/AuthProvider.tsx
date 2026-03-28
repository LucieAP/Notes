import { authApi } from "@/features/auth/api";
import {
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from "@/shared/api/tokenStorage";
import { AuthContext } from "@/shared/hooks/useAuth";
import { User } from "@/shared/types/user";
import { ReactNode, useCallback, useEffect, useMemo, useState } from "react";

function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;

    const fetchUser = async () => {
      try {
        let token = getAccessToken();

        if (!token) {
          const refreshRes = await authApi.refresh();
          token = refreshRes.token;
          setAccessToken(token);
        }

        const me = await authApi.getUser();
        if (isMounted) {
          setUser(me);
        }
      } catch {
        clearAccessToken();
        if (isMounted) {
          setUser(null);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchUser();

    return () => {
      isMounted = false;
    };
  }, []);

  const logout = useCallback(async () => {
    await authApi.logout();
    clearAccessToken();
    setUser(null);
  }, []);

  const value = useMemo(() => {
    return { user, loading, logout };
  }, [user, loading, logout]);

  return <AuthContext value={value}>{children}</AuthContext>;
}

export default AuthProvider;
