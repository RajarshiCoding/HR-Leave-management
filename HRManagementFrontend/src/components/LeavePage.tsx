import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";

export default function LeavePage() {
  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar></Sidebar>
      <div className="flex flex-col bg-gray-200 p-6 h-screen w-full">
        {/* Top bar */}
        <div className="flex items-center justify-between w-full h-16 bg-blue-600 px-6 rounded-tr-sm rounded-tl-sm">
          <BasicClock format12={true} />
          <button className="text-white font-medium">Right Button</button>
        </div>

        {/* Content area */}
        <div className="flex-1 h-full w-full bg-accent rounded-br-sm rounded-bl-sm">
          <p className="p-4">Main content area</p>
        </div>
      </div>
    </div>
  );
}
