import { useState } from "react";

function StepSection() {
  const [hovered, setHovered] = useState(false);
  return (
    <section
      className="w-full flex flex-col items-start gap-2 py-1"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <span className="text-[#888] text-sm min-w-[24px] text-right select-none shrink-0 pt-px">
        Шаги:
      </span>

      <div className="flex w-full">
        <textarea
          className="bg-transparent border-none outline-none text-white text-sm flex-1 resize-none overflow-hidden leading-relaxed placeholder-[#555]"
          placeholder="Введите шаг приготовления"
          rows={1}
        />

        <button
          className="text-[#666] hover:text-white text-xs px-1 py-0.5 rounded bg-transparent border-none cursor-pointer transition-all duration-150 shrink-0 mt-px"
          aria-label="Удалить шаг"
          style={{ opacity: hovered ? 1 : 0 }}
        >
          ✕
        </button>
      </div>

      <div className="flex gap-1 mt-2">
        <button className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150">
          <span>+</span> шаг
        </button>

        <button className="flex items-center gap-1 text-[#888] hover:text-white hover:bg-[#333] text-xs px-2 py-1 rounded bg-transparent border-none cursor-pointer transition-all duration-150">
          <span className="text-sm leading-none">+</span> подгруппа
        </button>
      </div>
    </section>
  );
}

export default StepSection;
