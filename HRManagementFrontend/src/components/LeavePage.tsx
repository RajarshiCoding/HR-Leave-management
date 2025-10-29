import { useEffect, useState } from "react";
import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";

import { LeaveRequestDialog } from "./ui/LeaveRequestDialog";

import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { Button } from "./ui/button";

import { Check, X } from "lucide-react";

export default function LeavePage() {
  const [employeeData, setEmployeeData] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  async function processLeaveRequest(
    requestId: number,
    status: "approved" | "rejected",
    hrNote?: string
  ): Promise<void> {
    // Validate status input
    if (status !== "approved" && status !== "rejected") {
      throw new Error("Status must be either 'approved' or 'rejected'.");
    }

    // Default HR note
    const note = hrNote?.trim() || "Processed leave!";

    // Get JWT token from localStorage
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("Authorization token not found in localStorage.");
    }

    try {
      const response = await fetch(
        `http://localhost:5062/api/leave/${requestId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            status: status,
            hrNote: note,
          }),
        }
      );

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to update leave: ${errorText}`);
      }

      console.log(`✅ Leave #${requestId} ${status} successfully.`);
    } catch (error) {
      console.error("❌ Error processing leave:", error);
    }
  }

  const fetchEmployeeData = async () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        setError("No token found in localStorage");
        return;
      }

      const response = await fetch("http://localhost:5062/api/leave", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      const data = await response.json();
      console.log(data);
      // console.log(data);
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
      <Sidebar></Sidebar>
      <div className="flex flex-col bg-gray-200 p-6 h-screen w-full">
        {/* Top bar */}
        <div className="flex items-center justify-between w-full h-16 bg-blue-600 px-6 rounded-tr-sm rounded-tl-sm">
          <BasicClock format12={true} />
        </div>

        {/* Content area */}
        <div className="flex-1 h-full w-full bg-accent rounded-br-sm rounded-bl-sm">
          {/* <p className="p-4">Main content area</p> */}
          {error ? (
            <h1>Error</h1>
          ) : (
            <Table>
              <TableCaption>A list of all employees.</TableCaption>
              <TableHeader>
                <TableRow>
                  {/* <TableHead className="w-[100px]">Invoice</TableHead> */}
                  <TableHead>Name</TableHead>
                  <TableHead className="text-center">Start Date</TableHead>
                  <TableHead className="text-center">Number of Days</TableHead>
                  <TableHead className="text-center">Action</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {employeeData.map((emp: any) => (
                  <TableRow>
                    <TableCell className="font-medium">{emp.name}</TableCell>
                    <TableCell className="text-center">
                      {emp.startDate.split("T")[0]}
                    </TableCell>
                    <TableCell className="text-center">
                      {emp.noOfDays}
                    </TableCell>
                    <TableCell className="text-center">
                      <LeaveRequestDialog
                        onClose={fetchEmployeeData}
                        requestId={emp.requestId}
                      />

                      <Button
                        onClick={() => {
                          processLeaveRequest(emp.requestId, "rejected");
                        }}
                        variant={"outline"}
                        className="text-red-600 border-red-600 hover:bg-red-300 hover:text-red-900"
                      >
                        <X />
                      </Button>

                      <Button
                        onClick={() => {
                          processLeaveRequest(emp.requestId, "approved");
                        }}
                        variant={"outline"}
                        className="text-green-600 border-green-600 hover:bg-green-300 hover:text-green-900"
                      >
                        <Check />
                      </Button>
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
