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
            var query = @"SELECT lr.*, e.""Name"" FROM leave_requests lr JOIN employees e ON lr.""EmpId"" = e.""EmpId"" WHERE lr.""Status"" = 'Pending';";
            try
            {
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<LeaveRequest>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
            }
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
            try
            {    
                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<LeaveRequest>(query, new { Id = requestId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
            }
        }

        // Get leave requests by employee ID
        public async Task<IEnumerable<LeaveRequest>> GetLeavesByEmployeeIdAsync(int empId)
        {
            var query = @"SELECT * FROM leave_requests WHERE ""EmpId"" = @Id";
            try
            {
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<LeaveRequest>(query, new { Id = empId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
            }
        }

        // Add new leave request
        public async Task<int> AddLeaveAsync(LeaveRequest leave)
        {
            if (leave.EndDate < leave.StartDate)
                throw new ArgumentException("End date must be greater than or equal to start date.");

            // ✅ Get all holidays
            var getHolidayquery = @"SELECT * FROM holidays ORDER BY ""Date""";
            // var checkQuery = @"SELECT ""NoOfDays"" FROM leave_requests WHERE ""RequestId"" = @Id;";
            var checkQuery = @"SELECT ""LeaveBalance"" FROM employees WHERE ""EmpId"" = @Id;";
            // ✅ Insert leave request
            var query = @"
                INSERT INTO leave_requests
                (""EmpId"", ""StartDate"", ""EndDate"", ""NoOfDays"", ""Reason"", ""Status"", ""AppliedOn"") 
                VALUES
                (@EmpId, @StartDate, @EndDate, @NoOfDays, @Reason, @Status, @AppliedOn) 
                RETURNING ""RequestId"";
            ";
            
            try
            {
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

                var checkNoOfDays = await connection.QuerySingleOrDefaultAsync<int>(checkQuery, new { Id = leave.EmpId });
                System.Console.WriteLine(checkNoOfDays);
                System.Console.WriteLine(workingDays);
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
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
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

            try
            {
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, leave);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
            }
        }


        public async Task<bool> UpdateLeaveCounterAsync(int requestId, int days)
        {
            var query = @"
               UPDATE employees
               SET ""LeaveBalance"" = ""LeaveBalance"" - @Days WHERE ""EmpId"" = (
               SELECT ""EmpId"" FROM leave_requests WHERE ""RequestId"" = @Id);";

            try
            {   
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Id = requestId, Days = days });
                // System.Console.WriteLine(affectedRows);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
            }
        }

        // Check if any pending requests exist
        public async Task<bool> HasPendingRequestsAsync()
        {
            var query = @"SELECT COUNT(*) FROM leave_requests WHERE ""Status"" = 'Pending'";
            try
            {   
                using var connection = _context.CreateConnection();
                var count = await connection.ExecuteScalarAsync<int>(query);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB Please try again later.", ex);
            }
        }
    }
}
