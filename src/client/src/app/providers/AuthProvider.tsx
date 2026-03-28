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
    const controller = new AbortController();

    const fetchUser = async () => {
      try {
        let token = getAccessToken();

        if (!token) {
          const refreshRes = await authApi.refresh({
            signal: controller.signal,
          });
          token = refreshRes.token;
          if (!controller.signal.aborted) {
            setAccessToken(token);
          }
        }

        const me = await authApi.getUser({ signal: controller.signal });
        if (!controller.signal.aborted) {
          setUser(me);
        }
      } catch {
        if (!controller.signal.aborted) {
          clearAccessToken();
          setUser(null);
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false);
        }
      }
    };

    fetchUser();

    return () => {
      controller.abort();
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
