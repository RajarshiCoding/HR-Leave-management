import React from "react";
import { Sidebar } from "./Sidebar";
import { Calendar } from "@/components/ui/calendar";

export default function CalendarPage() {
  const holidays = [
    new Date(2025, 0, 1), // Jan 1
    new Date(2025, 9, 26), // Jan 26
    new Date(2025, 2, 29), // Mar 29
  ];

  const [date, setDate] = React.useState<Date | undefined>(
    new Date(2025, 9, 28)
  );

  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar></Sidebar>
      <div className="flex flex-col bg-gray-200 p-6 h-screen w-full">
        {/* Content Section */}
        <Calendar
          mode="single"
          selected={date}
          onSelect={setDate}
          defaultMonth={date}
          className="rounded-lg border [--cell-size:--spacing(11)] md:[--cell-size:--spacing(12)]"
          buttonVariant="ghost"
          modifiers={{
            holiday: holidays,
          }}
          modifiersClassNames={{
            holiday:
              "relative before:content-['•'] text-3xl before:absolute before:text-red-500 before:bottom-[-5px] before:left-1/2 before:-translate-x-1/2",
          }}
        />
      </div>
    </div>
  );
}
