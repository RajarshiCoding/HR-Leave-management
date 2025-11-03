using Dapper;
using HRManagementBackend.Data;
using HRManagementBackend.Models;
using System.Data;

namespace HRManagementBackend.Services
{
    public class LeaveService
    {
        private readonly DapperContext _context;

        public LeaveService(DapperContext context)
        {
            _context = context;
        }

        // Get all leave requests
        public async Task<IEnumerable<LeaveRequest>> GetAllLeavesAsync()
        {
            var query = "SELECT lr.*, e.\"Name\" FROM leave_requests lr JOIN employees e ON lr.\"EmpId\" = e.\"EmpId\" WHERE lr.\"Status\" = 'Pending';";


            // Console.WriteLine(query);
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LeaveRequest>(query);
        }

        // Get leave by request ID
        public async Task<LeaveRequest?> GetLeaveByIdAsync(int requestId)
        {
            var query = @"
                        SELECT 
                            lr.*,
                            e.""Name"",
                            e.""Department""
                        FROM leave_requests lr
                        INNER JOIN employees e
                            ON lr.""EmpId"" = e.""EmpId""
                        WHERE lr.""RequestId"" = @Id;
                        ";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<LeaveRequest>(query, new { Id = requestId });
        }

        // Get leave requests by employee ID
        public async Task<IEnumerable<LeaveRequest>> GetLeavesByEmployeeIdAsync(int empId)
        {
            var query = @"SELECT * FROM leave_requests WHERE ""EmpId"" = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<LeaveRequest>(query, new { Id = empId });
        }

        // Add new leave request
        public async Task<int> AddLeaveAsync(LeaveRequest leave)
        {
            var query = @"
                INSERT INTO leave_requests
                (""EmpId"", ""StartDate"", ""EndDate"", ""Reason"", ""Status"", ""AppliedOn"")
                VALUES
                (@EmpId, @StartDate, @EndDate, @Reason, @Status, @AppliedOn)
                RETURNING ""RequestId"";
            ";


            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, leave);
        }

        // Update leave request status (approve/reject)
        public async Task<bool> UpdateLeaveStatusAsync(LeaveRequest leave)
        {
            var query = @"
                UPDATE leave_requests
                SET ""Status"" = @Status, ""HrNote"" = @HrNote, ""ReviewedOn"" = NOW()
                WHERE ""RequestId"" = @RequestId;
            ";


            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, leave);
            return affectedRows > 0;
        }


        public async Task<bool> UpdateLeaveCounterAsync(int requestId)
        {
            var query = @"
               UPDATE employees
               SET ""LeaveBalance"" = ""LeaveBalance"" - 1 WHERE ""EmpId"" = (
               SELECT ""EmpId"" FROM leave_requests WHERE ""RequestId"" = @Id);";


            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = requestId });
            return affectedRows > 0;
        }

        // Check if any pending requests exist
        public async Task<bool> HasPendingRequestsAsync()
        {
            var query = @"SELECT COUNT(*) FROM leave_requests WHERE ""Status"" = 'Pending'";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query);
            return count > 0;
        }
    }
}
