import { useState, useEffect } from "react";
import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

function EmpTrack() {
  const [leaveData, setLeaveData] = useState<any[]>([]);

  const [error, setError] = useState<string | null>(null);

  const fetchLeaveData = async () => {
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

      const response = await fetch(
        `http://localhost:5062/api/Leave/employee/${empId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      const data = await response.json();

      console.log(data, empId);

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      setLeaveData(data);
    } catch (err: any) {
      setError(err.message);
    }
  };

  useEffect(() => {
    fetchLeaveData();
  }, []);

  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar
        isAdmin={localStorage.getItem("role") == "HR" ? true : false}
      ></Sidebar>
      <div className="flex flex-col bg-gray-200 p-2 h-screen w-full">
        {/* Top bar */}
        <div className="flex items-center justify-between w-full h-16 bg-blue-600 px-6 rounded-tr-sm rounded-tl-sm">
          <BasicClock format12={true} />
        </div>
        {/* Content area */}
        <div className="flex-1 h-full w-full bg-accent rounded-br-sm rounded-bl-sm flex flex-col">
          {error ? (
            <h1>Error</h1>
          ) : (
            <Table>
              <TableCaption>A list of Leaves.</TableCaption>
              <TableHeader>
                <TableRow>
                  {/* <TableHead className="w-[100px]">Invoice</TableHead> */}
                  <TableHead>Applied on</TableHead>
                  <TableHead className="text-center">End Date</TableHead>
                  <TableHead className="text-center">HR Note</TableHead>
                  <TableHead className="text-center">Reason</TableHead>
                  <TableHead>Status</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {leaveData.map((emp: any) => (
                  <TableRow key={emp.requestId} className=" hover:bg-muted/50">
                    <TableCell className="font-medium">
                      {new Date(emp.appliedOn).toDateString()}
                    </TableCell>
                    <TableCell className="text-center">
                      {new Date(emp.endDate).toDateString()}
                    </TableCell>
                    <TableCell className="text-center">
                      {emp.hrNote
                        ? emp.hrNote.slice(0, 20) +
                          (emp.hrNote.length > 20 ? "..." : "")
                        : ""}
                    </TableCell>
                    <TableCell className="text-center">
                      {emp.reason
                        ? emp.reason.slice(0, 20) +
                          (emp.reason.length > 20 ? "..." : "")
                        : ""}
                    </TableCell>
                    <TableCell
                      className={
                        emp.status === "Approved"
                          ? "text-green-600"
                          : emp.status === "Pending"
                          ? "text-yellow-500"
                          : emp.status === "Rejected"
                          ? "text-red-500"
                          : ""
                      }
                    >
                      {emp.status}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </div>
      </div>
    </div>
  );
}

export default EmpTrack;
