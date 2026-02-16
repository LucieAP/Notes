import { createPortal } from "react-dom";
import SearchIcon from "../icons/SearchIcon";
import { useEffect, useRef } from "react";

interface Props {
  onClose: () => void;
}

function SearchModal({ onClose }: Props) {
  const inputRef = useRef<HTMLInputElement>(null);
  const backdropRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        onClose();
      }
    };

    window.addEventListener("keydown", handler);

    return () => window.removeEventListener("keydown", handler);
  }, []);

  return createPortal(
    <div className="relative z-10">
      {/* Backdrop - фон */}
      <div
        className="flex fixed inset-0 min-h-full justify-center text-center items-center rounded bg-neutral-700/10"
        ref={backdropRef}
        onMouseDown={(e) => {
          if (e.target === backdropRef.current) {
            onClose();
          }
        }}
      >
        {/* Dialog - окно модалки */}
        <div
          className="flex flex-col relative gap-5 max-w-5xl w-full max-h-200 min-h-200 h-full p-6 bg-neutral-900 text-left transform overflow-hidden rounded-lg shadow-xl outline -outline-offset-1 outline-white/10 transition-all"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="flex items-center border pl-4 gap-2 border-gray-500/30 h-12 rounded-full overflow-hidden w-full">
            <SearchIcon type={2} />
            <input
              type="text"
              ref={inputRef}
              placeholder="Search"
              className="w-full h-full outline-none bg-transparent placeholder-gray-500 text-sm select-none"
            />
          </div>

          {/* Content */}
          <div className="flex overflow-y-auto">
            Lorem ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit!Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit!Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit! Lorem ipsum dolor, sit amet consectetur
            adipisicing elit. Architecto molestias quos magnam, qui libero est
            porro asperiores voluptates obcaecati inventore debitis vel
            laboriosam quam praesentium, nobis animi, repellat amet fugit! Lorem
            ipsum dolor, sit amet consectetur adipisicing elit. Architecto
            molestias quos magnam, qui libero est porro asperiores voluptates
            obcaecati inventore debitis vel laboriosam quam praesentium, nobis
            animi, repellat amet fugit!
            yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
          </div>
          {/* Footer */}
          <div className="flex mt-auto">
            <button
              className="flex cursor-pointer bg-red-700 hover:bg-red-800 shadow-xl outline -outline-offset-1 outline-white/10 rounded-md p-1"
              onClick={onClose}
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>,
    document.body,
  );
}

export default SearchModal;
