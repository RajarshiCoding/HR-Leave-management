using Dapper;
using HRManagementBackend.Data;
using HRManagementBackend.Models;
using System.Data;

namespace HRManagementBackend.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DapperContext _context;

        public EmployeeService(DapperContext context)
        {
            _context = context;
        }

        // Get all employees
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var query = @"
                        SELECT 
                        e.*,
                        CASE 
                            WHEN EXISTS (
                                SELECT 1
                                FROM leave_requests lr
                                WHERE lr.""EmpId"" = e.""EmpId""
                                AND lr.""Status"" = 'Approved'
                                AND CURRENT_DATE BETWEEN lr.""StartDate"" AND lr.""EndDate""
                            )
                            THEN 'On Leave'
                            ELSE 'In Office'
                        END AS ""Status""
                    FROM employees e;";
            try
            {
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<Employee>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch employees. Please try again later.", ex);
            }
        }

        // Get employee by ID
        public async Task<Employee?> GetEmployeeByIdAsync(int empId)
        {
            var query = @"
                        SELECT 
                        e.*,
                        CASE 
                            WHEN EXISTS (
                                SELECT 1
                                FROM leave_requests lr
                                WHERE lr.""EmpId"" = e.""EmpId""
                                AND lr.""Status"" = 'Approved'
                                AND CURRENT_DATE BETWEEN lr.""StartDate"" AND lr.""EndDate""
                            )
                            THEN 'On Leave'
                            ELSE 'In Office'
                        END AS ""Status""
                    FROM employees e
                    WHERE e.""EmpId"" = @Id;
                                ";
            try
            {
                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = empId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch employee details. Please try again later.", ex);
            }
        }

        // Add new employee
        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            var checkQuery = @"SELECT COUNT(*) FROM employees WHERE ""Email"" = @Email";
            var query = @"
                INSERT INTO employees 
                    (""Name"", ""Email"", ""PasswordHash"", ""Department"", ""Designation"", ""Contact"", ""DOB"")
                VALUES
                    (@Name, @Email, @PasswordHash, @Department, @Designation, @Contact, @DOB)
                RETURNING ""EmpId"";";
            try
            {
                using var connection = _context.CreateConnection();

                var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { employee.Email });
                if (exists > 0)
                    return -1;

                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.PasswordHash);
                return await connection.ExecuteScalarAsync<int>(query, employee);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add employee. Please try again later.", ex);
            }
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
                    ""DOB"" = @DOB
                WHERE ""EmpId"" = @EmpId;
            ";
            try
            {
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, employee);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update employee. Please try again later.", ex);
            }
        }

        // Delete employee
        public async Task<bool> DeleteEmployeeAsync(int empId)
        {
            var query = @"DELETE FROM employees WHERE ""EmpId"" = @Id";
            try
            {
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = empId });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to delete employee. Please try again later.", ex);
            }
        }
    }
}
