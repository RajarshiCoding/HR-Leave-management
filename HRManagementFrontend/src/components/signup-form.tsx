import { useState, type FormEvent } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field";

import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { Input } from "@/components/ui/input";

import { Contact, Eye, EyeOff } from "lucide-react";

const departments = [
  "Software Development",
  "Quality Assurance",
  "DevOps",
  "UI/UX Design",
  "Product Management",
  "IT Support",
  "Database Administration",
  "Network & Infrastructure",
  "Cybersecurity",
  "Data Science",
  "Project Management",
  "Customer Support",
  "Client Relations",
  "Training & Learning",
];

export function SignupForm({
  onSigninSuccess,
}: {
  onSigninSuccess?: () => void;
}) {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleSigninRedirect = () => {
    navigate("/");
  };

  const handleSignin = async (e: FormEvent<HTMLDivElement>) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await axios.post("http://localhost:5062/api/auth/register", {
        name,
        email,
        password,
        department: "Human Resource", // 🔒 constant
        designation: "HR", // 🔒 constant
        contact: phone,
      });
      if (res.status == 200) {
        const res2 = await axios.post("http://localhost:5062/api/auth/login", {
          email,
          password,
        });
        localStorage.setItem("token", res2.data.token);
        localStorage.setItem("name", name);
        localStorage.setItem("role", res.data.designation);
        onSigninSuccess?.();
        setTimeout(() => {
          setLoading(false);
          {
            if (res.data.designation === "HR") {
              navigate("/dashboard/admin");
            } else if (res.data.designation === "Employee") {
              navigate("/dashboard/employee");
            }
          }
        }, 1000);
      }
    } catch (err: any) {
      console.log(err.response?.data?.message || "Login failed");
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Create an account</CardTitle>
        <CardDescription>
          Enter your information below to create your account
        </CardDescription>
      </CardHeader>
      <CardContent onSubmit={handleSignin}>
        <form>
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="name">Full Name</FieldLabel>
              <Input
                id="name"
                type="text"
                placeholder="Give your full name"
                required
                onChange={(e) => setName(e.target.value)}
              />
            </Field>
            <Field>
              <FieldLabel htmlFor="email">Email</FieldLabel>
              <Input
                id="email"
                type="email"
                placeholder="yourname@example.com"
                required
                onChange={(e) => setEmail(e.target.value)}
              />
            </Field>

            <Field>
              <FieldLabel htmlFor="phone">Phone Number</FieldLabel>
              <Input
                id="phone"
                type="tel"
                placeholder="9876543210"
                pattern="[0-9]{10}"
                maxLength={10}
                required
                onChange={(e) => setPhone(e.target.value)}
              />
            </Field>

            {/* <Field>
              <FieldLabel htmlFor="role">Department</FieldLabel>
              <Select>
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Choose your department" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectLabel>Departments</SelectLabel>
                    {departments.map((dept) => (
                      <SelectItem key={dept} value={dept}>
                        {dept}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
            </Field> */}

            <Field>
              <FieldLabel htmlFor="password">Password</FieldLabel>
              <div className="flex items-center gap-2">
                <Input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  onClick={() => setShowPassword(!showPassword)}
                >
                  {showPassword ? (
                    <EyeOff className="w-5 h-5" />
                  ) : (
                    <Eye className="w-5 h-5" />
                  )}
                </Button>
              </div>
            </Field>
            {/* <Field>
              <FieldLabel htmlFor="confirm-password">
                Confirm Password
              </FieldLabel>
              <Input id="confirm-password" type="password" required />
              <FieldDescription>Please confirm your password.</FieldDescription>
            </Field> */}

            <Field>
              <Button type="submit" disabled={loading}>
                {loading ? "Creating account...." : "Create Account"}
              </Button>
              <FieldDescription className="px-6 text-center">
                Already have an account?{" "}
                <a href="#" onClick={() => handleSigninRedirect()}>
                  Log in
                </a>
              </FieldDescription>
            </Field>
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  );
}
