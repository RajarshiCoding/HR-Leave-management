import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Eye } from "lucide-react";

export function LeaveRequestDialog({
  requestId,
  processLeaveRequest,
}: {
  requestId: number;
  processLeaveRequest?: (
    requestId: number,
    status: "Approved" | "Rejected",
    hrNote?: string
  ) => void;
}) {
  const [leave, setLeave] = useState<any>(null);
  const [hrNote, setHrNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);

  // async function processLeaveRequests(
  //   requestId: number,
  //   status: "Approved" | "Rejected",
  //   hrNote?: string
  // ): Promise<void> {
  //   // Validate status input
  //   if (status !== "Approved" && status !== "Rejected") {
  //     throw new Error("Status must be either 'approved' or 'rejected'.");
  //   }

  //   // Default HR note
  //   const note = hrNote?.trim() || "Processed leave!";

  //   // Get JWT token from localStorage
  //   const token = localStorage.getItem("token");
  //   if (!token) {
  //     throw new Error("Authorization token not found in localStorage.");
  //   }

  //   try {
  //     const response = await fetch(
  //       `http://localhost:5062/api/leave/${requestId}`,
  //       {
  //         method: "PUT",
  //         headers: {
  //           "Content-Type": "application/json",
  //           Authorization: `Bearer ${token}`,
  //         },
  //         body: JSON.stringify({
  //           status: status,
  //           hrNote: note,
  //         }),
  //       }
  //     );

  //     if (!response.ok) {
  //       throw new Error(`Failed to update leave: ${await response.text()}`);
  //     } else {
  //       const update = await fetch(
  //         `http://localhost:5062/api/leave/update/${requestId}`,
  //         {
  //           method: "PUT",
  //           headers: {
  //             "Content-Type": "application/json",
  //             Authorization: `Bearer ${token}`,
  //           },
  //         }
  //       );
  //       if (!update.ok) {
  //         throw new Error(
  //           `Failed to update leave count: ${await update.text()}`
  //         );
  //       }
  //     }

  //     console.log(`✅ Leave #${requestId} ${status} successfully.`);
  //   } catch (error) {
  //     console.error("❌ Error processing leave:", error);
  //   }
  // }

  const fetchLeaveDetails = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("token");
      const res = await fetch(`http://localhost:5062/api/leave/${requestId}`, {
        method: "GET",
        headers: { Authorization: `Bearer ${token}` },
      });
      const data = await res.json();
      setLeave(data);
    } catch (err) {
      console.error("Error fetching leave details:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog
      open={open}
      onOpenChange={(isOpen) => {
        setOpen(isOpen);
        if (isOpen) {
          fetchLeaveDetails();
        } else {
          // onClose?.(); // 👈 call parent refresh when dialog closes
        }
      }}
    >
      <DialogTrigger asChild>
        <Button
          variant={"outline"}
          className="text-blue-600 border-blue-600 hover:bg-blue-300 hover:text-blue-900"
          onClick={() => setOpen(true)}
        >
          <Eye />
        </Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Leave Request Details</DialogTitle>
          <DialogDescription>Review this leave request</DialogDescription>
        </DialogHeader>

        {loading ? (
          <p className="text-center py-4">Loading...</p>
        ) : leave ? (
          <div className="space-y-2 text-sm">
            <p>
              <strong>Employee Name:</strong> {leave.name || "N/A"}
            </p>
            <p>
              <strong>Employee ID:</strong> {leave.empId}
            </p>
            <p>
              <strong>Department:</strong> {leave.department || "N/A"}
            </p>
            <p>
              <strong>Start Date:</strong>{" "}
              {new Date(leave.startDate).toLocaleDateString()}
            </p>
            <p>
              <strong>End Date:</strong>{" "}
              {new Date(leave.endDate).toLocaleDateString()}
            </p>
            <p>
              <strong>Number of Days:</strong> {leave.noOfDays}
            </p>
            <p>
              <strong>Reason:</strong> {leave.reason}
            </p>

            <div>
              <label className="block font-medium text-gray-700">
                HR Note:
              </label>
              <Input
                placeholder="Add an optional note"
                value={hrNote}
                onChange={(e) => setHrNote(e.target.value)}
              />
            </div>
          </div>
        ) : (
          <p className="text-center py-4 text-gray-500">No data found.</p>
        )}

        <DialogFooter className="flex justify-between mt-4">
          <Button
            variant="destructive"
            onClick={async () => {
              await processLeaveRequest!(requestId, "Rejected", hrNote);
              setOpen(false); // 👈 close after reject
              // onClose?.();
            }}
            disabled={loading}
          >
            Reject
          </Button>
          <Button
            className="bg-green-600 hover:bg-green-700"
            onClick={async () => {
              await processLeaveRequest!(requestId, "Approved", hrNote);
              setOpen(false); // 👈 close after approve
              // onClose?.();
            }}
            disabled={loading}
          >
            Accept
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
