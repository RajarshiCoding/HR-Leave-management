import React from "react";
import { Sidebar } from "./Sidebar";
import { Calendar } from "@/components/ui/calendar";
import { Button } from "./ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

export default function CalendarPage() {
  const [holidays, setHolidays] = React.useState<any[]>([]);
  const [holidayList, setHolidayList] = React.useState<any[]>([]);
  const [holidayTitle, setHolidayTitle] = React.useState<string>("Holiday");
  const [date, setDate] = React.useState<Date | undefined>(
    new Date(2025, 9, 28)
  );
  const [isHoliday, setIsHoliday] = React.useState<boolean>(false);

  const fetchHolidays = async () => {
    try {
      const res = await fetch("http://localhost:5062/api/Holiday");
      const data = await res.json();

      setHolidays(data);
      const parsedDates = data.map((h: any) => new Date(h.date));
      setHolidayList(parsedDates);
    } catch (error) {
      console.error("Error fetching holidays:", error);
    }
  };
  React.useEffect(() => {
    fetchHolidays();
  }, []);

  // ✅ Function to check if selected date is a holiday
  const checkIfHoliday = (selectedDate?: Date) => {
    if (!selectedDate) return;

    const match = holidays.find((holiday) => {
      const holidayDate = new Date(holiday.date);
      return (
        holidayDate.getDate() === selectedDate.getDate() &&
        holidayDate.getMonth() === selectedDate.getMonth() &&
        holidayDate.getFullYear() === selectedDate.getFullYear()
      );
    });

    setIsHoliday(!!match);
    setHolidayTitle(match ? match.title : "");
  };

  // ✅ Run check whenever user selects a new date
  React.useEffect(() => {
    checkIfHoliday(date);
    // console.log(holidayTitle);
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
            holiday: holidayList,
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
                  <DialogTitle>Add holiday</DialogTitle>
                  <DialogDescription>
                    Select the date to be added
                  </DialogDescription>
                </DialogHeader>

                {/* 🗓️ Form for adding holiday */}
                <form
                  onSubmit={async (e) => {
                    e.preventDefault();

                    if (!date) return alert("Please select a date first!");

                    const formData = new FormData(e.currentTarget);
                    const title = formData.get("title") as string;
                    const description = formData.get("description") as string;

                    if (!title.trim() || !description.trim()) {
                      alert("Title and Description are required.");
                      return;
                    }

                    try {
                      const res = await fetch(
                        "http://localhost:5062/api/Holiday",
                        {
                          method: "POST",
                          headers: {
                            "Content-Type": "application/json",
                          },
                          body: JSON.stringify({
                            title,
                            description,
                            date: new Date(date.getTime() + 19800000)
                              .toISOString()
                              .split("T")[0],
                          }),
                        }
                      );

                      if (!res.ok) throw new Error("Failed to add holiday");

                      const newHoliday = await res.json();

                      // ✅ Update holiday lists
                      // setHolidays((prev) => [...prev, newHoliday]);
                      // setHolidayList((prev) => [
                      //   ...prev,
                      //   new Date(newHoliday.date),
                      // ]);

                      fetchHolidays();

                      alert("Holiday added successfully!");
                    } catch (error) {
                      console.error("Error adding holiday:", error);
                    }
                  }}
                  className="space-y-4"
                >
                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Date:
                    </label>
                    <p className="text-gray-800 font-semibold">
                      {date ? date.toDateString() : "No date selected"}
                    </p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Title:
                    </label>
                    <input
                      name="title"
                      type="text"
                      required
                      className="mt-1 w-full rounded-md border border-gray-300 p-2"
                      placeholder="Enter holiday title"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Description:
                    </label>
                    <textarea
                      name="description"
                      required
                      className="mt-1 w-full rounded-md border border-gray-300 p-2"
                      placeholder="Enter holiday description"
                    />
                  </div>

                  <DialogFooter>
                    <DialogClose asChild>
                      <Button variant="outline">Cancel</Button>
                    </DialogClose>
                    <Button type="submit">Save changes</Button>
                  </DialogFooter>
                </form>
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
                  {isHoliday ? holidayTitle : ""}
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
