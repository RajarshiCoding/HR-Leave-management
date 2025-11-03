 
namespace HRManagementBackend.Models
{
    public class Employee
    {
        public int EmpId { get; set; }                // Primary key
        public required string Name { get; set; }              // Employee full name
        public required string Email { get; set; }             // Unique email
        public string? PasswordHash { get; set; }      // Hashed password
        public string? PasswordSalt { get; set; }      // Salt for password hashing
        public required string Department { get; set; }        // Department name
        public required string Designation { get; set; }      // Role or designation
        public required string Contact { get; set; }          // Contact number
        public DateTime JoiningDate { get; set; }     // Joining date
        public int LeaveBalance { get; set; }         // Remaining leaves
        public int LeaveTaken { get; set; }           // Leaves taken
        public string? Status { get; set; }            // Active / Inactive / On Leave
        public DateTime  DOB { get; set; }     // date of birth 
    }

    public class EmployeeUpdateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? Contact { get; set; }
        public int? LeaveBalance { get; set; }
        public int? LeaveTaken { get; set; }
        public string? Status { get; set; }
    }
    public class EmployeeLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

}
