import { Outlet } from "react-router-dom";

function ContentArea() {
  return (
    <div className="flex-1 overflow-y-auto ">
      <div className="max-w-4xl mx-auto px-24 py-12">
        <Outlet />
      </div>
    </div>
  );
}

export default ContentArea;
