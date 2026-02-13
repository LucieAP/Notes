function HeaderPage() {
  return (
    <div className="flex flex-col px-2 pb-5">
      <div className="flex items-center gap-2 px-2 py-1 rounded-lg hover:bg-neutral-700 cursor-pointer transition-colors select-none">
        <div className="w-5 h-5 rounded bg-neutral-700 flex items-center justify-center text-white text-xs font-medium">
          P
        </div>
        <span className="text-sm font-medium">Profile</span>
      </div>
    </div>
  );
}

export default HeaderPage;
