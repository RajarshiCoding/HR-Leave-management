using Dapper;
using HRManagementBackend.Data;
using HRManagementBackend.Models;
using System.Data;

namespace HRManagementBackend.Services
{
    public class EmployeeService
    {
        private readonly DapperContext _context;

        public EmployeeService(DapperContext context)
        {
            _context = context;
        }

        // Get all employees
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var query = "SELECT * FROM employees";              //not all, just the data needed
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Employee>(query);
        }

        // Get employee by ID
        public async Task<Employee?> GetEmployeeByIdAsync(int empId)
        {
            var query = @"SELECT * FROM employees WHERE ""EmpId"" = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = empId });
        }

        // Add new employee
        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            // var query = @"
            //     INSERT INTO employees 
            //     (name, email, password_hash, password_salt, department, designation, contact, joining_date, leave_balance, leave_taken, status)
            //     VALUES
            //     (@Name, @Email, @PasswordHash, @PasswordSalt, @Department, @Designation, @Contact, @JoiningDate, @LeaveBalance, @LeaveTaken, @Status)
            //     RETURNING emp_id;
            // ";
            var query = @"
                    INSERT INTO employees 
                    (""Name"", ""Email"", ""PasswordHash"", ""PasswordSalt"", ""Department"", ""Designation"", ""Contact"", ""JoiningDate"", ""LeaveBalance"", ""LeaveTaken"", ""Status"")
                    VALUES
                    (@Name, @Email, @PasswordHash, @PasswordSalt, @Department, @Designation, @Contact, @JoiningDate, @LeaveBalance, @LeaveTaken, 'Active')
                    RETURNING ""EmpId"";
                ";


            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, employee);
        }

        // Update employee info
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
        var query = @"
            UPDATE employees SET
                ""Name"" = @Name,
                ""Email"" = @Email,
                ""Department"" = @Department,
                ""Designation"" = @Designation,
                ""Contact"" = @Contact,
                ""LeaveBalance"" = @LeaveBalance,
                ""LeaveTaken"" = @LeaveTaken,
                ""Status"" = @Status
            WHERE ""EmpId"" = @EmpId;
        ";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, employee);
            return affectedRows > 0;
        }

        // Delete employee
        public async Task<bool> DeleteEmployeeAsync(int empId)
        {
            var query = @"DELETE FROM employees WHERE ""EmpId"" = @Id";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = empId });
            return affectedRows > 0;
        }
    }
}
