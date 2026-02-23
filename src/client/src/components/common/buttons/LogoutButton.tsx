import LogoutIcon from "../icons/LogoutIcon";

function LogoutButton({ onLogout }: { onLogout: () => void }) {
  return (
    <button type="button" onClick={onLogout} className="ml-auto cursor-pointer">
      <LogoutIcon />
    </button>
  );
}

export default LogoutButton;
