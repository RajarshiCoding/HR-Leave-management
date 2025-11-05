using Dapper;
using HRManagementBackend.Data;
using HRManagementBackend.Models;
using System.Data;

namespace HRManagementBackend.Services
{
    public class LeaveService : ILeaveService
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
            if (leave.EndDate < leave.StartDate)
                throw new ArgumentException("End date must be greater than or equal to start date.");

            // ✅ Get all holidays
            var getHolidayquery = @"SELECT * FROM holidays ORDER BY ""Date""";
            var checkQuery = @"SELECT ""NoOfDays"" FROM leave_requests WHERE ""RequestId"" = @Id;";
            // ✅ Insert leave request
            var query = @"
                INSERT INTO leave_requests
                (""EmpId"", ""StartDate"", ""EndDate"", ""NoOfDays"", ""Reason"", ""Status"", ""AppliedOn"") 
                VALUES
                (@EmpId, @StartDate, @EndDate, @NoOfDays, @Reason, @Status, @AppliedOn) 
                RETURNING ""RequestId"";
            ";

            using var connection = _context.CreateConnection();

            var holidays = await connection.QueryAsync<Holiday>(getHolidayquery);

            var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();

            // ✅ Calculate working days
            int workingDays = 0;
            DateTime currentDate = leave.StartDate.Date;

            while (currentDate <= leave.EndDate.Date)
            {
                bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday ||
                                currentDate.DayOfWeek == DayOfWeek.Sunday;

                bool isHoliday = holidayDates.Contains(currentDate);

                if (!isWeekend && !isHoliday)
                    workingDays++;

                currentDate = currentDate.AddDays(1);
            }

            var checkNoOfDays = await connection.QuerySingleOrDefaultAsync<int>(checkQuery, new { Id = leave.RequestId });
            if(checkNoOfDays >= workingDays)
            {
                // ✅ Set computed days in the leave model
                leave.NoOfDays = workingDays;
                leave.AppliedOn = DateTime.Now;
                return await connection.ExecuteScalarAsync<int>(query, leave);
            }
            else
            {
                return -1;
            }
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


        public async Task<bool> UpdateLeaveCounterAsync(int requestId, int days)
        {
            var query = @"
               UPDATE employees
               SET ""LeaveBalance"" = ""LeaveBalance"" - @Days WHERE ""EmpId"" = (
               SELECT ""EmpId"" FROM leave_requests WHERE ""RequestId"" = @Id);";


            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = requestId, Days = days });
            // System.Console.WriteLine(affectedRows);
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
