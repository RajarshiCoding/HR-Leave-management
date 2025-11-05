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
  const [holidayTitle, setHolidayTitle] = React.useState<string>("");
  const [holidayDescription, setHolidayDescription] =
    React.useState<string>("");
  const [date, setDate] = React.useState<Date | undefined>(
    new Date(new Date().setHours(0, 0, 0, 0))
  );

  const [isHoliday, setIsHoliday] = React.useState<boolean>(false);

  const fetchHolidays = async () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.log("No token found in localStorage");
        return;
      }
      const res = await fetch("http://localhost:5062/api/Holiday", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
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
  }, [date]);

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
    setHolidayDescription(match ? match.description : "");
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
          modifiers={{
            holiday: holidayList,
            weekend: (day) => day.getDay() === 0 || day.getDay() === 6,
          }}
          showOutsideDays={false}
          modifiersClassNames={{
            holiday:
              "text-red-500 font-extrabold rounded-md bg-red-200 hover:!bg-red-200",
            weekend:
              "bg-pink-100 text-pink-800 font-semibold hover:!bg-pink-100 rounded-md",
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

                      // const newHoliday = await res.json();

                      // ✅ Update holiday lists
                      // setHolidays((prev) => [...prev, newHoliday]);
                      // setHolidayList((prev) => [
                      //   ...prev,
                      //   new Date(newHoliday.date),
                      // ]);

                      setHolidayTitle(title);

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

            <Dialog>
              <DialogTrigger asChild>
                <Button variant={"outline"} disabled={!isHoliday}>
                  Edit
                </Button>
              </DialogTrigger>
              <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                  <DialogTitle>Edit holiday</DialogTitle>
                  <DialogDescription>Edit the contents</DialogDescription>
                </DialogHeader>
                <form
                  onSubmit={async (e) => {
                    e.preventDefault();
                    if (!date) return alert("Please select a date first!");
                    console.log(
                      new Date(date.getTime() + 19800000)
                        .toISOString()
                        .split("T")[0]
                    );
                    const formData = new FormData(e.currentTarget);
                    const title =
                      (formData.get("title") as string) || holidayTitle;
                    const description =
                      (formData.get("description") as string) ||
                      holidayDescription;

                    // if (!title.trim()) {
                    //   title = holidayTitle;
                    // }

                    try {
                      const res = await fetch(
                        `http://localhost:5062/api/Holiday/${
                          new Date(date.getTime() + 19800000)
                            .toISOString()
                            .split("T")[0]
                        }`,
                        {
                          method: "PUT",
                          headers: {
                            "Content-Type": "application/json",
                          },
                          body: JSON.stringify({
                            title,
                            description,
                          }),
                        }
                      );

                      if (!res.ok) throw new Error("Failed to add holiday");

                      fetchHolidays();

                      alert("Holiday edited successfully!");
                    } catch (error) {
                      console.error("Error adding holiday:", error);
                    }
                  }}
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
                      className="mt-1 w-full rounded-md border border-gray-300 p-2"
                      placeholder={holidayTitle}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Description:
                    </label>
                    <textarea
                      name="description"
                      className="mt-1 w-full rounded-md border border-gray-300 p-2"
                      placeholder={holidayDescription}
                    />
                  </div>
                  <DialogFooter>
                    <DialogClose asChild>
                      <Button variant="outline">Cancel</Button>
                    </DialogClose>
                    <Button type="submit">Edit changes</Button>
                  </DialogFooter>
                </form>
              </DialogContent>
            </Dialog>

            <Dialog>
              <DialogTrigger asChild>
                <Button variant={"outline"} disabled={!isHoliday}>
                  Delete
                </Button>
              </DialogTrigger>
              <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                  <DialogTitle>Delete Holiday</DialogTitle>
                  <DialogDescription>
                    Are you sure you want to delete this holiday? This action
                    cannot be undone.
                  </DialogDescription>
                </DialogHeader>

                <div className="space-y-3">
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
                    <p className="text-gray-800 font-semibold">
                      {holidayTitle}
                    </p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Description:
                    </label>
                    <p className="text-gray-800 font-semibold">
                      {holidayDescription}
                    </p>
                  </div>
                </div>

                <DialogFooter>
                  <DialogClose asChild>
                    <Button variant="outline">Cancel</Button>
                  </DialogClose>
                  <Button
                    variant="destructive"
                    onClick={async () => {
                      if (!date) return alert("Please select a date first!");

                      try {
                        const res = await fetch(
                          `http://localhost:5062/api/Holiday/${
                            new Date(date.getTime() + 19800000)
                              .toISOString()
                              .split("T")[0]
                          }`,
                          {
                            method: "DELETE",
                          }
                        );

                        if (!res.ok)
                          throw new Error("Failed to delete holiday");

                        fetchHolidays();
                        alert("Holiday deleted successfully!");
                      } catch (error) {
                        console.error("Error deleting holiday:", error);
                      }
                    }}
                  >
                    Delete
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
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
