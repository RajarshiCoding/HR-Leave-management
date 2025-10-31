import { useEffect, useState } from "react";
import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";

function EmpDashboard() {
  const [employeeData, setEmployeeData] = useState<any | null>(null);
  const [error, setError] = useState<string | null>(null);

  const fetchEmployeeData = async () => {
    try {
      const token = localStorage.getItem("token");
      const empId = localStorage.getItem("empId");

      if (!token) {
        setError("No token found in localStorage");
        return;
      }
      if (!empId) {
        setError("No employee ID found in localStorage");
        return;
      }

      console.log("Emp id", empId);

      const response = await fetch(
        `http://localhost:5062/api/employee/${empId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      const data = await response.json();
      console.log(data);

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      setEmployeeData(data);
    } catch (err: any) {
      setError(err.message);
    }
  };

  useEffect(() => {
    fetchEmployeeData();
  }, []);

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
                <strong>Leave Balance:</strong>
                <p>{employeeData?.leaveBalance ?? "N/A"}</p>
              </div>
              <div className="flex-1 flex-col bg-white m-2 rounded-lg flex items-center justify-center">
                <strong>Leave Taken:</strong>
                <p>{employeeData?.leaveTaken ?? "N/A"}</p>
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
