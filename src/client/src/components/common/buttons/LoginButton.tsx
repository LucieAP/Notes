import GoogleLogo from "../icons/GoogleLogo";

interface LoginButtonProps {
  onClick?: () => void;
}

function LoginButton({ onClick }: LoginButtonProps) {
  return (
    <>
      <div className="flex items-center justify-center">
        <button
          type="button"
          onClick={onClick}
          className="flex px-4 py-2 border gap-2 select-none cursor-pointer border-gray-400 rounded-lg text-neutral-700 dark:text-slate-200 hover:border-slate-400 dark:hover:border-slate-500 hover:text-slate-900 dark:hover:text-neutral-300 hover:shadow transition duration-150"
        >
          <GoogleLogo className="w-6 h-6" />
          <span className="text-lg">Login with Google</span>
        </button>
      </div>
    </>
  );
}

export default LoginButton;
