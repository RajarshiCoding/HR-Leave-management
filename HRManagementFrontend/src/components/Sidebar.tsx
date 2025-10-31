import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { useLocation, useNavigate } from "react-router-dom";
import { Home, CalendarDays, Plane, LogOut, ClipboardList } from "lucide-react";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

interface SidebarProps {
  isAdmin?: boolean;
}
export function Sidebar({ isAdmin = true }: SidebarProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const [isAny, setisAny] = useState<Boolean>();
  const [empId, setEmpId] = useState<string | null>(null);

  const handleLogout = () => {
    const confirmLogout = window.confirm("Are you sure you want to log out?");
    if (confirmLogout) {
      localStorage.clear();
      sessionStorage.clear();
      navigate("/", { replace: false });
    }
  };
  useEffect(() => {
    const empId = localStorage.getItem("empId");
    setEmpId(empId);
    const fetchEmployeeData = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) {
          console.log("No token found in localStorage");
          return;
        }

        const isAnyResponse = await fetch(
          "http://localhost:5062/api/leave/isAny",
          { method: "GET" }
        );

        if (!isAnyResponse.ok) {
          throw new Error(
            `Error ${isAnyResponse.status}: ${isAnyResponse.statusText}`
          );
        }

        const data = await isAnyResponse.json();
        console.log(data, data.hasPending);
        setisAny(data.hasPending);

        // const data = await response.json();
        // console.log("Employee data:", data);
      } catch (err: any) {
        console.error("Fetch error:", err.message);
      }
    };

    fetchEmployeeData();
  }, []);

  const username = localStorage.getItem("name") || "User";

  return (
    <aside className="h-screen w-64 bg-white shadow-md flex flex-col justify-between py-6">
      {/* Top Section */}
      <div>
        <div className="px-6 pb-6 text-2xl font-semibold text-gray-800 border-b">
          Hi, {username}
        </div>

        {/* Navigation Buttons */}
        <div className="flex flex-col mt-6 space-y-2 px-4">
          {isAdmin ? (
            <>
              <Button
                variant={
                  location.pathname === "/dashboard/admin" ? "default" : "ghost"
                }
                className="justify-start"
                onClick={() => navigate("/dashboard/admin")}
              >
                <Home className="mr-2 h-5 w-5" /> Home
              </Button>

              <Button
                variant={location.pathname === "/leave" ? "default" : "ghost"}
                className={`justify-start ${
                  isAny ? "text-red-500 hover:text-red-600" : ""
                }`}
                onClick={() => navigate("/leave")}
              >
                <Plane className="mr-2 h-5 w-5" /> Leave
              </Button>

              <Button
                variant={
                  location.pathname === "/calendar" ? "default" : "ghost"
                }
                className="justify-start"
                onClick={() => navigate("/calendar")}
              >
                <CalendarDays className="mr-2 h-5 w-5" /> Calendar
              </Button>
            </>
          ) : (
            // employee part
            <>
              <Button
                variant={
                  location.pathname === "/dashboard/employee"
                    ? "default"
                    : "ghost"
                }
                className="justify-start"
                onClick={() => {
                  navigate("/dashboard/employee");
                  console.log(empId);
                }}
              >
                <Home className="mr-2 h-5 w-5" /> Home
              </Button>

              <Button
                variant={location.pathname === "/track" ? "default" : "ghost"}
                className="justify-start"
                onClick={() => navigate("/track")}
              >
                <ClipboardList className="mr-2 h-5 w-5" /> Track
              </Button>
            </>
          )}
        </div>
      </div>

      {/* Bottom Profile Icon */}
      <div className="px-6">
        <Tooltip>
          <TooltipTrigger>
            <Button
              variant="outline"
              className="rounded-full w-12 h-12 p-0 flex items-center justify-center"
              onClick={handleLogout}
            >
              {/* <UserRound className="h-6 w-6" /> */}
              <LogOut className="h-6 w-6" />
            </Button>
          </TooltipTrigger>
          <TooltipContent>
            <p>Click to log out</p>
          </TooltipContent>
        </Tooltip>
      </div>
    </aside>
  );
}
