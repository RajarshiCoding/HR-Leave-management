import * as React from "react";
import { format } from "date-fns";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Textarea } from "@/components/ui/textarea";
import { CalendarIcon } from "lucide-react";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from "@/lib/utils";

interface LeaveRequestDialogProps {
  open: boolean;
  onClose: () => void;
}

export function LeaveRequestDialog({ open, onClose }: LeaveRequestDialogProps) {
  const [startDate, setStartDate] = React.useState<Date>(
    new Date(new Date().setDate(new Date().getDate() + 1))
  );
  const [endDate, setEndDate] = React.useState<Date>(
    new Date(new Date().setDate(new Date().getDate() + 1))
  );
  const [reason, setReason] = React.useState("");

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const handleSubmit = async () => {
    const empId = localStorage.getItem("empId");
    if (!empId) {
      alert("Employee ID not found in localStorage!");
      return;
    }

    const leaveData = {
      empId,
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString(),
      reason,
      status: "Pending",
      appliedOn: new Date().toISOString(),
    };

    try {
      const token = localStorage.getItem("token");
      const response = await fetch("http://localhost:5062/api/leave", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : "",
        },
        body: JSON.stringify(leaveData),
      });

      if (!response.ok) throw new Error(`Error ${response.status}`);

      alert("Leave submitted successfully!");
      onClose(); // close dialog
      window.location.reload(); // refresh page
    } catch (error: any) {
      console.error(error);
      alert("Failed to submit leave.");
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Submit Leave Request</DialogTitle>
        </DialogHeader>

        {/* Start Date */}
        <div className="space-y-2">
          <label className="text-sm font-medium">Start Date</label>
          <Popover>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  "w-full justify-start text-left font-normal",
                  !startDate && "text-muted-foreground"
                )}
              >
                <CalendarIcon className="mr-2 h-4 w-4" />
                {startDate ? format(startDate, "PPP") : "Pick a date"}
              </Button>
            </PopoverTrigger>
            <PopoverContent align="start" className="p-0">
              <Calendar
                mode="single"
                selected={startDate}
                modifiers={{
                  weekend: (day) => day.getDay() === 0 || day.getDay() === 6,
                }}
                modifiersClassNames={{
                  weekend:
                    "bg-pink-100 text-pink-800 font-semibold hover:!bg-pink-100 rounded-md",
                }}
                onSelect={(date) =>
                  date && setStartDate(date) && setEndDate(date)
                }
                disabled={(date) => date <= today}
              />
            </PopoverContent>
          </Popover>
        </div>

        {/* End Date */}
        <div className="space-y-2">
          <label className="text-sm font-medium">End Date</label>
          <Popover>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  "w-full justify-start text-left font-normal",
                  !endDate && "text-muted-foreground"
                )}
              >
                <CalendarIcon className="mr-2 h-4 w-4" />
                {endDate ? format(endDate, "PPP") : "Pick a date"}
              </Button>
            </PopoverTrigger>
            <PopoverContent align="start" className="p-0">
              <Calendar
                mode="single"
                selected={endDate}
                modifiers={{
                  weekend: (day) => day.getDay() === 0 || day.getDay() === 6,
                }}
                modifiersClassNames={{
                  weekend:
                    "bg-pink-100 text-pink-800 font-semibold hover:!bg-pink-100 rounded-md",
                }}
                onSelect={(date) => date && setEndDate(date)}
                disabled={(date) => date < startDate}
              />
            </PopoverContent>
          </Popover>
        </div>

        {/* Reason */}
        <div className="space-y-2">
          <label className="text-sm font-medium">Reason</label>
          <Textarea
            placeholder="Enter reason for leave..."
            value={reason}
            onChange={(e) => setReason(e.target.value)}
          />
        </div>

        {/* Submit Button */}
        <Button onClick={handleSubmit} className="w-full mt-4">
          Submit Leave
        </Button>
      </DialogContent>
    </Dialog>
  );
}
