import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { useLocation, useNavigate } from "react-router-dom";
import {
  Home,
  CalendarDays,
  Plane,
  LogOut,
  ClipboardList,
  List,
  LockKeyholeOpen,
} from "lucide-react";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";

interface SidebarProps {
  isAdmin?: boolean;
}

export function Sidebar({ isAdmin = true }: SidebarProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const [isAny, setisAny] = useState<Boolean>();
  const [empId, setEmpId] = useState<string | null>(null);

  // State for change password dialog
  const [changePassOpen, setChangePassOpen] = useState(false);
  const [oldPass, setOldPass] = useState("");
  const [newPass, setNewPass] = useState("");
  const [loading, setLoading] = useState(false);

  const handleLogout = () => {
    const confirmLogout = window.confirm("Are you sure you want to log out?");
    if (confirmLogout) {
      localStorage.clear();
      sessionStorage.clear();
      navigate("/", { replace: false });
    }
  };

  const handleChangePass = async () => {
    setChangePassOpen(true);
  };

  const submitChangePass = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("token");
      const id = localStorage.getItem("empId");
      const res = await fetch("http://localhost:5062/api/auth/changepass", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          empId: id,
          oldPassword: oldPass,
          newPassword: newPass,
        }),
      });

      if (res.ok) {
        setChangePassOpen(false);
        handleLogout(); // logout on success
      } else {
        setChangePassOpen(false); // just close on
      }
    } catch (err) {
      console.error(err);
      setChangePassOpen(false);
    } finally {
      setLoading(false);
      setOldPass("");
      setNewPass("");
    }
  };

  useEffect(() => {
    const empId = localStorage.getItem("empId");
    setEmpId(empId);
    const fetchEmployeeData = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) return;

        const isAnyResponse = await fetch(
          "http://localhost:5062/api/leave/isAny",
          {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (!isAnyResponse.ok)
          throw new Error(
            `Error ${isAnyResponse.status}: ${isAnyResponse.statusText}`
          );

        const data = await isAnyResponse.json();
        setisAny(data.hasPending);
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

              <Button
                variant={
                  location.pathname === "/dashboard/employee"
                    ? "default"
                    : "ghost"
                }
                className="justify-start"
                onClick={() => navigate("/dashboard/employee")}
              >
                <List className="mr-2 h-5 w-5" /> Details
              </Button>

              <Button
                variant={location.pathname === "/track" ? "default" : "ghost"}
                className="justify-start"
                onClick={() => navigate("/track")}
              >
                <ClipboardList className="mr-2 h-5 w-5" /> Track
              </Button>
            </>
          ) : (
            <>
              <Button
                variant={
                  location.pathname === "/dashboard/employee"
                    ? "default"
                    : "ghost"
                }
                className="justify-start"
                onClick={() => navigate("/dashboard/employee")}
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

      {/* Bottom Profile Icons */}
      <div className="px-6 flex gap-2">
        {/* Logout */}
        <Tooltip>
          <TooltipTrigger>
            <Button
              variant="outline"
              className="rounded-full w-12 h-12 p-0 flex items-center justify-center"
              onClick={handleLogout}
            >
              <LogOut className="h-6 w-6" />
            </Button>
          </TooltipTrigger>
          <TooltipContent>
            <p>Click to log out</p>
          </TooltipContent>
        </Tooltip>

        {/* Change Password */}
        <Tooltip>
          <TooltipTrigger>
            <Button
              variant="outline"
              className="rounded-full w-12 h-12 p-0 flex items-center justify-center"
              onClick={handleChangePass}
            >
              <LockKeyholeOpen className="h-6 w-6" />
            </Button>
          </TooltipTrigger>
          <TooltipContent>
            <p>Change Password</p>
          </TooltipContent>
        </Tooltip>
      </div>

      {/* Change Password Dialog */}
      <Dialog open={changePassOpen} onOpenChange={setChangePassOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Change Password</DialogTitle>
          </DialogHeader>
          <div className="flex flex-col gap-2">
            <Input
              type="password"
              placeholder="Old Password"
              value={oldPass}
              onChange={(e) => setOldPass(e.target.value)}
            />
            <Input
              type="password"
              placeholder="New Password"
              value={newPass}
              onChange={(e) => setNewPass(e.target.value)}
            />
          </div>
          <DialogFooter className="mt-4 flex justify-end gap-2">
            <Button
              variant="outline"
              onClick={() => setChangePassOpen(false)}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button onClick={submitChangePass} disabled={loading}>
              Change
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </aside>
  );
}
