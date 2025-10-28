import React from "react";
import { Sidebar } from "./Sidebar";
import { Calendar } from "@/components/ui/calendar";
import { Button } from "./ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

export default function CalendarPage() {
  const holidays = [
    new Date(2025, 0, 1), // Jan 1
    new Date(2025, 9, 26), // Jan 26
    new Date(2025, 2, 29), // Mar 29
  ];

  const [date, setDate] = React.useState<Date | undefined>(
    new Date(2025, 9, 28)
  );
  const [isHoliday, setIsHoliday] = React.useState<boolean>(false);

  // ✅ Function to check if selected date is a holiday
  const checkIfHoliday = (selectedDate: Date | undefined) => {
    if (!selectedDate) return;
    const match = holidays.some(
      (holiday) =>
        holiday.getDate() === selectedDate.getDate() &&
        holiday.getMonth() === selectedDate.getMonth() &&
        holiday.getFullYear() === selectedDate.getFullYear()
    );
    setIsHoliday(match);
  };

  // ✅ Run check whenever user selects a new date
  React.useEffect(() => {
    checkIfHoliday(date);
  }, [date]);

  return (
    <div className="flex min-h-screen bg-gray-500 ">
      <Sidebar></Sidebar>
      <div className="flex bg-gray-200 p-6 h-screen w-full justify-evenly">
        {/* Content Section */}
        <Calendar
          mode="single"
          selected={date}
          onSelect={setDate}
          defaultMonth={date}
          className="rounded-lg border [--cell-size:--spacing(11)] md:[--cell-size:--spacing(12)] "
          buttonVariant="ghost"
          modifiers={{
            holiday: holidays,
          }}
          modifiersClassNames={{
            holiday:
              "relative before:content-['•'] text-3xl before:absolute before:text-red-500 before:bottom-[-5px] before:left-1/2 before:-translate-x-1/2",
          }}
        />

        {/* 📝 Info Section */}
        <div className="w-full md:w-1/3 bg-white rounded-lg shadow-lg p-6 flex flex-col justify-between items-center mt-6 md:mt-0 md:ml-6">
          <div>
            <Dialog>
              <DialogTrigger asChild>
                <Button variant={"outline"}>Add</Button>
              </DialogTrigger>
              <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                  <DialogTitle>Edit profile</DialogTitle>
                  <DialogDescription>
                    Make changes to your profile here. Click save when
                    you&apos;re done.
                  </DialogDescription>
                </DialogHeader>
              </DialogContent>
            </Dialog>
            <Button variant={"outline"} disabled={!isHoliday}>
              Edit
            </Button>
            <Button variant={"outline"} disabled={!isHoliday}>
              Delete
            </Button>
          </div>

          <div className="flex flex-col justify-center items-center">
            {date ? (
              <>
                <h2 className="text-lg font-semibold text-gray-700">
                  {date.toDateString()}
                </h2>
                <p
                  className={`mt-3 text-center text-lg font-medium ${
                    isHoliday ? "text-green-600" : "text-gray-500"
                  }`}
                >
                  {isHoliday ? "🎉 This is a holiday!" : ""}
                </p>
              </>
            ) : (
              <p className="text-gray-500">Select a date to see details.</p>
            )}
          </div>
          <div></div>
        </div>
      </div>
    </div>
  );
}
