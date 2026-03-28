import { useSearchParams } from "react-router-dom";
import LoginButton from "../common/buttons/LoginButton";
import FailedAuthIcon from "../common/icons/FailedAuthIcon";

function LoginPage() {
  const [searchParams] = useSearchParams();
  const error = searchParams.get("error");

  const handleLogin = () => {
    window.location.href = `${import.meta.env.VITE_BASE_URL}/auth/google-login`;
  };

  return (
    <>
      <div className="min-h-screen flex items-center justify-center p-4">
        <div className="flex flex-col p-7 mx-4 max-w-md min-w-xs text-center border-0.5 border-neutral-300 rounded-4xl bg-neutral-700 space-y-2">
          {/* Auth error alert */}
          {error && (
            <div
              className="bg-red-50 border border-red-200 text-sm text-red-800 rounded-lg p-4 dark:bg-red-500/20 dark:border-red-900 dark:text-red-400"
              role="alert"
              tabIndex={-1}
              aria-labelledby="hs-with-list-label"
            >
              <div className="flex">
                <div className="shrink-0">
                  <FailedAuthIcon />
                </div>
                <div className="ms-4">
                  <h3 id="hs-with-list-label" className="text-sm font-semibold">
                    Auth failed.
                  </h3>
                  {/* <div className="mt-2 text-sm text-red-800 dark:text-red-300">
                  <ul className="list-disc space-y-1 ps-5">
                    <li>This username is already in use</li>
                    <li>Email field can't be empty</li>
                    <li>Please enter a valid phone number</li>
                  </ul>
                </div> */}
                </div>
              </div>
            </div>
          )}

          <div className="flex flex-col items-center gap-3">
            <h2 className="text-xl">Sign in to your account</h2>
            <LoginButton onClick={handleLogin} />
          </div>
        </div>
      </div>
    </>
  );
}

export default LoginPage;
