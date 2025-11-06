// VarsDialog.tsx
import { useState, useEffect } from "react";
import axios from "axios";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";

export default function VarsDialog({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (v: boolean) => void;
}) {
  const [vars, setVars] = useState<{ [key: string]: number }>({});
  const [form, setForm] = useState<{ [key: string]: string }>({});
  const [loading, setLoading] = useState(false);

  // Fetch variables when dialog opens
  useEffect(() => {
    if (open) {
      const token = localStorage.getItem("token");
      setLoading(true);

      fetch("http://localhost:5062/api/vars", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      })
        .then(async (res) => {
          if (!res.ok) {
            throw new Error(`HTTP error! Status: ${res.status}`);
          }
          const data = await res.json();

          const result: Record<string, number> = {};
          data.forEach((v: any) => {
            result[v.varName] = v.value;
          });

          setVars(result);
          setForm(
            Object.fromEntries(
              Object.entries(result).map(([k, v]) => [k, String(v)])
            )
          );
        })
        .catch((err) => console.error("Error fetching vars:", err))
        .finally(() => setLoading(false));
    }
  }, [open]);

  const handleChange = (key: string, value: string) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = async () => {
    const token = localStorage.getItem("token");
    const finalData = Object.keys(vars).map((key) => ({
      varName: key,
      value: Number(form[key]) || vars[key],
    }));

    try {
      await axios.post("http://localhost:5062/api/vars", finalData, {
        headers: { Authorization: `Bearer ${token}` },
      });
      alert("Variables updated successfully!");
      onOpenChange(false);
    } catch (err) {
      console.error(err);
      alert("Failed to update variables.");
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>System Variables</DialogTitle>
        </DialogHeader>

        {loading ? (
          <p>Loading...</p>
        ) : (
          <div className="space-y-4 mt-4">
            {Object.entries(vars).map(([key, val]) => (
              <div key={key} className="space-y-2">
                <Label htmlFor={key}>{key}</Label>
                <Input
                  id={key}
                  type="number"
                  placeholder={String(val)}
                  value={form[key] ?? ""}
                  min={0}
                  onChange={(e) => handleChange(key, e.target.value)}
                />
              </div>
            ))}

            <Button onClick={handleSubmit} className="w-full mt-4">
              Save Changes
            </Button>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
