import SearchIcon from "../icons/SearchIcon";
import { useState } from "react";
import SearchModal from "../modals/SearchModal";

function SearchButton() {
  const [open, setOpen] = useState(false);

  const handleClose = () => {
    setOpen(false);
    console.log("нажата");
  };

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        type="button"
        className="flex items-center w-full rounded-lg px-2 py-1 space-x-2 hover:bg-neutral-700 cursor-pointer"
      >
        <SearchIcon type={1} />
        <span>Search</span>
      </button>
      {open && <SearchModal onClose={handleClose} />}
    </>
  );
}

export default SearchButton;
