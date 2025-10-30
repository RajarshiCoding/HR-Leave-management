import { useEffect, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Loader2 } from "lucide-react";

interface Employee {
  empId: number;
  name: string;
  email: string;
  passwordHash: string;
  passwordSalt: string;
  department: string;
  designation: string;
  contact: string;
  joiningDate: string;
  leaveBalance: number;
  leaveTaken: number;
  status: string;
}

interface EmployeeDetailProps {
  empId: number;
  open: boolean;
  onClose: () => void;
}

export function EmployeeDetail({ empId, open, onClose }: EmployeeDetailProps) {
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return; // only fetch when dialog is open
    const fetchEmployee = async () => {
      setLoading(true);
      setError(null);

      try {
        const token = localStorage.getItem("token");
        if (!token) throw new Error("No token found");

        const res = await fetch(`http://localhost:5062/api/Employee/${empId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) throw new Error(`Error ${res.status}: ${res.statusText}`);

        const data: Employee = await res.json();
        setEmployee(data);
      } catch (err: any) {
        setError(err.message || "Failed to load employee details");
      } finally {
        setLoading(false);
      }
    };

    fetchEmployee();
  }, [empId, open]);

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="max-w-lg rounded-2xl">
        <DialogHeader>
          <DialogTitle>Employee Details</DialogTitle>
          <DialogDescription>
            Detailed information for employee #{empId}
          </DialogDescription>
        </DialogHeader>

        {loading ? (
          <div className="flex items-center justify-center py-10">
            <Loader2 className="h-6 w-6 animate-spin mr-2" />
            Loading...
          </div>
        ) : error ? (
          <p className="text-red-600 text-center">{error}</p>
        ) : employee ? (
          <div className="grid grid-cols-2 gap-3 py-4 text-sm">
            <p className="font-medium">Name:</p>
            <p>{employee.name}</p>

            <p className="font-medium">Email:</p>
            <p>{employee.email}</p>

            <p className="font-medium">Department:</p>
            <p>{employee.department}</p>

            <p className="font-medium">Designation:</p>
            <p>{employee.designation}</p>

            <p className="font-medium">Contact:</p>
            <p>{employee.contact}</p>

            <p className="font-medium">Joining Date:</p>
            <p>
              {new Date(employee.joiningDate).toLocaleDateString("en-IN", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </p>

            <p className="font-medium">Leave Balance:</p>
            <p>{employee.leaveBalance}</p>

            <p className="font-medium">Leave Taken:</p>
            <p>{employee.leaveTaken}</p>

            <p className="font-medium">Status:</p>
            <p
              className={
                employee.status === "Active"
                  ? "text-green-600 font-medium"
                  : "text-red-500 font-medium"
              }
            >
              {employee.status}
            </p>

            <p className="font-medium">Password Hash:</p>
            <p className="truncate">{employee.passwordHash}</p>

            <p className="font-medium">Password Salt:</p>
            <p className="truncate">{employee.passwordSalt}</p>
          </div>
        ) : (
          <p className="text-center text-gray-500 py-8">No employee found</p>
        )}

        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
