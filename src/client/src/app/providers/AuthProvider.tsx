import { authApi } from "@/features/auth/api";
import { AuthContext } from "@/shared/hooks/useAuth";
import { User } from "@/shared/types/user";
import { ReactNode, useCallback, useEffect, useMemo, useState } from "react";

function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUser = async () => {
      const token = await cookieStore.get("token");

      if (!token?.value) {
        setLoading(false);
        return;
      }

      try {
        const me = await authApi.getUser();
        setUser(me);
      } catch {
        cookieStore.delete("token");
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, []);

  const logout = useCallback(async () => {
    await cookieStore.delete("token");
    setUser(null);
  }, []);

  const value = useMemo(() => {
    return { user, loading, logout };
  }, [user, loading, logout]);

  return <AuthContext value={value}>{children}</AuthContext>;
}

export default AuthProvider;
