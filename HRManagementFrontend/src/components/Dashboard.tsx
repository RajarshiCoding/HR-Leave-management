import React, { useEffect, useState } from "react";

import { Sidebar } from "./Sidebar";
import BasicClock from "./ui/clock";

import { CirclePlus } from "lucide-react";

import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useNavigate } from "react-router-dom";

const Dashboard: React.FC = () => {
  const [employeeData, setEmployeeData] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  const navigate = useNavigate();

  useEffect(() => {
    const fetchEmployeeData = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) {
          setError("No token found in localStorage");
          return;
        }

        const response = await fetch("http://localhost:5062/api/employee", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const data = await response.json();
        // console.log(data);
        if (!response.ok) {
          throw new Error(`Error ${response.status}: ${response.statusText}`);
        }

        setEmployeeData(data);
      } catch (err: any) {
        setError(err.message);
      }
    };

    fetchEmployeeData();
  }, []);

  const GetEmp = (EmpID: number) => {
    console.log("Clicked employee ID:", EmpID);
    navigate(`/dashboard/${EmpID}`);
    // you can fetch, set state, navigate, etc.
  };

  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar></Sidebar>
      <div className="flex flex-col bg-gray-200 p-6 h-screen w-full">
        {/* Top bar */}
        <div className="flex items-center justify-between w-full h-16 bg-blue-600 px-6 rounded-tr-sm rounded-tl-sm">
          <BasicClock format12={true} />
          <button className="text-white font-medium cursor-pointer">
            <CirclePlus size={42} color="black" />
          </button>
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
                  <TableHead className="text-center">Department</TableHead>
                  <TableHead className="text-center">Leave Balance</TableHead>
                  <TableHead className="text-center">Leave Taken</TableHead>
                  <TableHead>Status</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {employeeData.map((emp: any) => (
                  <TableRow onClick={() => GetEmp(emp.empId)}>
                    <TableCell className="font-medium">{emp.name}</TableCell>
                    <TableCell className="text-center">
                      {emp.department}
                    </TableCell>
                    <TableCell className="text-center">
                      {emp.leaveBalance}
                    </TableCell>
                    <TableCell className="text-center">
                      {emp.leaveTaken}
                    </TableCell>
                    <TableCell
                      className={
                        emp.status == "Active"
                          ? "text-green-600"
                          : "text-red-500"
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
};

export default Dashboard;
