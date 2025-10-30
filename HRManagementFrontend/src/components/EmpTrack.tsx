import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";

function EmpTrack() {
  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar isAdmin={false}></Sidebar>
    </div>
  );
}

export default EmpTrack;
