import { useEffect, useState } from "react";

type Props = {
  /** If true, show 12-hour format with AM/PM; otherwise 24-hour */
  format12?: boolean;
};

export default function BasicClock({ format12 = false }: Props) {
  const [now, setNow] = useState<Date>(new Date());

  useEffect(() => {
    const id = setInterval(() => setNow(new Date()), 1000);
    return () => clearInterval(id);
  }, []);

  const hours = now.getHours();
  const minutes = String(now.getMinutes()).padStart(2, "0");
  const seconds = String(now.getSeconds()).padStart(2, "0");
  const ampm = hours >= 12 ? "PM" : "AM";

  const displayHours = format12
    ? String(hours % 12 || 12).padStart(2, "0")
    : String(hours).padStart(2, "0");

  return (
    <div className="flex  h-full w-full">
      <div className="flex items-center m-1 p-2 rounded-xl border-2 border-black bg-blue-300">
        <div className="font-mono text-3xl md:text-4xl">
          {displayHours}:{minutes}:{seconds}
        </div>
        {format12 && <div className="text-sm font-medium">{ampm}</div>}
      </div>
    </div>
  );
}
