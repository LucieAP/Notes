import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";

function CallbackPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  useEffect(() => {
    const token = searchParams.get("token");

    if (token) {
      localStorage.setItem("token", token);
      navigate("/", { replace: true });
    } else {
      navigate("/login?error=auth_failed", { replace: true });
    }
  }, []);

  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-center">
        <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-neutral-600 mx-auto" />
        <p className="mt-4 text-gray-500">Signing in...</p>
      </div>
    </div>
  );
}

export default CallbackPage;
