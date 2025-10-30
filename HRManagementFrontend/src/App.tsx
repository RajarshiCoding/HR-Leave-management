import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import { useEffect, useState } from "react";
import { LoginForm } from "./components/login-form";
import { SignupForm } from "./components/signup-form";
import Dashboard from "./components/Dashboard";
import LeavePage from "./components/LeavePage";
import CalendarPage from "./components/CalendarPage";
import AddEmployees from "./components/AddEmployees";
import EmpDashboard from "./components/EmpDashboard";
import EmpTrack from "./components/EmpTrack";
// import EmployeeDetail from "./components/EmployeeDetail";

function App() {
  const [authChecked, setAuthChecked] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userRole, setUserRole] = useState<string | null>(null);

  const checkAuth = async () => {
    const token = localStorage.getItem("token");
    if (!token) {
      setAuthChecked(true);
      setIsAuthenticated(false);
      console.log(isAuthenticated, authChecked, "First");
      return;
    }

    try {
      const res = await fetch("http://localhost:5062/api/auth", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (res.status === 200) {
        const data = await res.json();
        setIsAuthenticated(true);
        setUserRole(data.role); // 👈 store backend role
      } else {
        setIsAuthenticated(false);
      }
    } catch (err) {
      console.error("Auth check failed:", err);
      setIsAuthenticated(false);
      console.log("Auth fail");
    } finally {
      setAuthChecked(true);
    }
  };
  useEffect(() => {
    checkAuth();
  }, []);
  useEffect(() => {
    checkAuth();
  }, [isAuthenticated]);

  if (!authChecked) return <div>Loading...</div>;

  return (
    <Router>
      <Routes>
        {/* Public Routes */}
        <Route
          path="/"
          element={
            <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
              <div className="w-full max-w-sm">
                <LoginForm
                  onLoginSuccess={() => {
                    checkAuth();
                    console.log(isAuthenticated, authChecked);
                  }}
                />
              </div>
            </div>
          }
        />
        <Route
          path="/register"
          element={
            <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
              <div className="w-full max-w-sm">
                <SignupForm
                  onSigninSuccess={() => {
                    checkAuth();
                    console.log(isAuthenticated, authChecked);
                  }}
                />
              </div>
            </div>
          }
        />

        {/* Role-based Protected Routes */}
        {isAuthenticated ? (
          <>
            {/* HR Routes */}
            {userRole === "HR" && (
              <>
                <Route path="/dashboard/admin" element={<Dashboard />} />
                <Route path="/leave" element={<LeavePage />} />
                <Route path="/calendar" element={<CalendarPage />} />
                <Route
                  path="/dashboard/admin/addEmp"
                  element={<AddEmployees />}
                />
              </>
            )}
            {/* Employee Routes */}
            {userRole === "Employee" && (
              <>
                {/* <Route path="/calendar" element={<CalendarPage />} /> */}
                <Route path="/dashboard/employee" element={<EmpDashboard />} />
                <Route path="/track" element={<EmpTrack />} />
              </>
            )}
            {/* Anything else redirects to home */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </>
        ) : (
          // Not logged in
          <Route path="*" element={<Navigate to="/" replace />} />
        )}
      </Routes>
    </Router>
  );
}

export default App;
