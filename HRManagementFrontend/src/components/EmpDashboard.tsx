import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";
import { useParams } from "react-router-dom";

function EmpDashboard() {
  const { empId } = useParams<{ empId: string }>();
  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar isAdmin={false}></Sidebar>
      <div className="flex flex-col bg-gray-200 p-2 h-screen w-full">
        {/* Top bar */}
        <div className="flex items-center justify-between w-full h-16 bg-blue-600 px-6 rounded-tr-sm rounded-tl-sm">
          <BasicClock format12={true} />
        </div>
        {/* Content area */}
        <div className="flex-1 h-full w-full bg-accent rounded-br-sm rounded-bl-sm flex flex-col">
          {/* Top Section */}
          <div className="flex flex-row flex-1">
            {/* Left Div */}
            <div className="flex-1 bg-white m-2 rounded-lg shadow-md flex items-center justify-center">
              <div className="flex-1 flex-col bg-white m-2 rounded-lg flex items-center justify-center">
                <p>Available</p>
                <p>12</p>
              </div>
              <div className="flex-1 flex-col bg-white m-2 rounded-lg flex items-center justify-center">
                <p>Token</p>
                <p>3</p>
              </div>
            </div>

            {/* Right Div */}
            <div className="flex-1 bg-white m-2 rounded-lg shadow-md flex items-center justify-center">
              <p>Top Right</p>
            </div>
          </div>

          {/* Bottom Section */}
          <div className="flex-1 bg-white m-2 rounded-lg shadow-md flex items-center justify-center">
            <p>Bottom Section</p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default EmpDashboard;
