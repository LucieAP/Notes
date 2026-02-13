import { NavLink } from "react-router-dom";
import Header from "./Header";
import DropdownIcon from "./icons/DropdownIcon";
import NoteIcon from "./icons/NoteIcon";
import RecipeIcon from "./icons/RecipeIcon";
import SearchIcon from "./icons/SearchIcon";
import TaskIcon from "./icons/TaskIcon";
import WorkspaceIcon from "./icons/WorkspaceIcon";
import { useState } from "react";
import Trash from "./Trash";
import Settings from "./Settings";

function Sidebar() {
  const [isOpen, setIsOpen] = useState(true);

  // function handleDropdownClick() {
  //   isCollapsed = !isCollapsed;
  // }

  return (
    <div className="w-60 h-screen border-r border-neutral-800 shrink-0 bg-neutral-800">
      <div className="flex flex-col h-full p-2 space-y-0.5">
        <Header />
        <div className="flex flex-col items-left px-1 rounded select-none text-sm transition-colors gap-0">
          <NavLink
            to="/workspace"
            className={({ isActive }) => `flex rounded-lg px-2 py-1 space-x-2
            ${isActive ? "bg-neutral-700" : "hover:bg-neutral-700"}`}
          >
            <WorkspaceIcon />
            <span>Workspace</span>
          </NavLink>

          <NavLink
            to="/search"
            className={({ isActive }) => `flex rounded-lg px-2 py-1 space-x-2
            ${isActive ? "bg-neutral-700" : "hover:bg-neutral-700"}`}
          >
            <SearchIcon />
            <span>Search</span>
          </NavLink>
          <div className="flex flex-col mt-5">
            <button
              onClick={() => setIsOpen((prev) => !prev)}
              className="flex rounded-lg hover:bg-neutral-700 px-1 py-1 space-x-2 text-xs cursor-pointer"
            >
              Private
              <DropdownIcon isOpen={isOpen} />
            </button>

            {isOpen && (
              <>
                {/* <DropdownIcon /> */}
                <NavLink
                  to="/notes"
                  className={({
                    isActive,
                  }) => `flex rounded-lg px-2 py-1 space-x-2
              ${isActive ? "bg-neutral-700" : "hover:bg-neutral-700"}`}
                >
                  <NoteIcon />
                  <span>Notes</span>
                </NavLink>

                <NavLink
                  to="/tasks"
                  className={({
                    isActive,
                  }) => `flex rounded-lg px-2 py-1 space-x-2
              ${isActive ? "bg-neutral-700" : "hover:bg-neutral-700"}`}
                >
                  <TaskIcon />
                  <span>Tasks</span>
                </NavLink>
                <NavLink
                  to="/recipes"
                  className={({
                    isActive,
                  }) => `flex rounded-lg px-2 py-1 space-x-2
              ${isActive ? "bg-neutral-700" : "hover:bg-neutral-700"}`}
                >
                  <RecipeIcon />
                  <span>Recipes</span>
                </NavLink>
              </>
            )}
          </div>
        </div>

        <div className="flex flex-col items-left px-1 rounded select-none text-sm transition-colors gap-0 mt-auto mb-20">
          <Settings />
          <Trash />
        </div>
      </div>
    </div>
  );
}

export default Sidebar;
