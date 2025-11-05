// src/components/HolidayDialog.tsx
"use client";

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
import { ScrollArea } from "./scroll-area";
// import { ScrollArea } from "@radix-ui/react-scroll-area";
// import { ScrollArea } from "@/components/scroll-area";

interface Holiday {
  holidayId: number;
  title: string;
  description: string;
  date: string;
  createdAt: string;
}

interface HolidayDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function HolidayDialog({
  open,
  onOpenChange,
}: HolidayDialogProps) {
  const [holidays, setHolidays] = useState<Holiday[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open) {
      setLoading(true);

      const token = localStorage.getItem("token");
      if (!token) {
        console.log("No token found in localStorage");
        return;
      }

      fetch("http://localhost:5062/api/Holiday", {
        headers: {
          "Content-Type": "application/json",
          Authorization: token ? `Bearer ${token}` : "",
        },
      })
        .then((res) => {
          if (!res.ok) {
            throw new Error(`HTTP error! status: ${res.status}`);
          }
          return res.json();
        })
        .then((data) => setHolidays(data))
        .catch((err) => console.error("Error fetching holidays:", err))
        .finally(() => setLoading(false));
    }
  }, [open]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Holiday List</DialogTitle>
          <DialogDescription>
            Upcoming and past holidays from the HR system
          </DialogDescription>
        </DialogHeader>

        <ScrollArea className="h-80 mt-4">
          {loading ? (
            <p className="text-center py-4">Loading...</p>
          ) : (
            <table className="w-full border-collapse">
              <thead>
                <tr className="text-left border-b">
                  <th className="py-2 px-3">Title</th>
                  <th className="py-2 px-3">Description</th>
                  <th className="py-2 px-3">Date</th>
                </tr>
              </thead>
              <tbody>
                {holidays.map((h) => (
                  <tr key={h.holidayId} className="border-b hover:bg-gray-50">
                    <td className="py-2 px-3 font-medium">{h.title}</td>
                    <td className="py-2 px-3">{h.description}</td>
                    <td className="py-2 px-3 text-sm text-gray-600">
                      {new Date(h.date).toLocaleDateString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </ScrollArea>

        <DialogFooter>
          <Button
            variant="default"
            onClick={() => onOpenChange(false)}
            className="cursor-pointer"
          >
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
