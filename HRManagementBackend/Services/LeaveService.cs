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

            var getHolidayQuery = @"SELECT * FROM holidays ORDER BY ""Date"";";
            var getMaxLeaveDaysQuery = @"SELECT ""value"" FROM customVar WHERE ""varName"" = 'MaxLeaveDays';";
            var checkBalanceQuery = @"SELECT ""LeaveBalance"" FROM employees WHERE ""EmpId"" = @Id;";

            var overlapCheckQuery = @"
                SELECT COUNT(*) 
                FROM leave_requests 
                WHERE ""EmpId"" = @EmpId
                AND ""Status"" IN ('Pending', 'Approved')
                AND (""StartDate"" <= @EndDate AND ""EndDate"" >= @StartDate);
            ";

            var insertQuery = @"
                INSERT INTO leave_requests
                (""EmpId"", ""StartDate"", ""EndDate"", ""NoOfDays"", ""Reason"", ""Status"", ""AppliedOn"") 
                VALUES
                (@EmpId, @StartDate, @EndDate, @NoOfDays, @Reason, @Status, @AppliedOn) 
                RETURNING ""RequestId"";
            ";

            try
            {
                using var connection = _context.CreateConnection();

                var holidays = await connection.QueryAsync<Holiday>(getHolidayQuery);
                var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();

                int maxAllowedDays = await connection.QueryFirstOrDefaultAsync<int>(getMaxLeaveDaysQuery);
                if (maxAllowedDays <= 0)
                {
                    maxAllowedDays = 10; 
                    Console.WriteLine("No valid MaxLeaveDays found in customVar. Using default value: 10");
                }
                
                int workingDays = 0;
                DateTime currentDate = leave.StartDate.Date;
                while (currentDate <= leave.EndDate.Date)
                {
                    bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;
                    bool isHoliday = holidayDates.Contains(currentDate);

                    if (!isWeekend && !isHoliday)
                        workingDays++;

                    currentDate = currentDate.AddDays(1);
                }

                
                int overlapping = await connection.ExecuteScalarAsync<int>(overlapCheckQuery, new
                {
                    EmpId = leave.EmpId,
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate
                });

                if (overlapping > 0)
                {
                    return -1;
                }

                
                int currentBalance = await connection.QuerySingleOrDefaultAsync<int>(checkBalanceQuery, new { Id = leave.EmpId });

                
                if (currentBalance >= workingDays && workingDays <= maxAllowedDays)
                {
                    leave.NoOfDays = workingDays;
                    leave.AppliedOn = DateTime.Now;

                    return await connection.ExecuteScalarAsync<int>(insertQuery, leave);
                }
                else
                {
                    
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to connect with DB. Please try again later.", ex);
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
                SET 
                    ""LeaveBalance"" = ""LeaveBalance"" - @Days,
                    ""LeaveTaken""   = ""LeaveTaken"" + @Days
                WHERE ""EmpId"" = (
                    SELECT ""EmpId"" FROM leave_requests WHERE ""RequestId"" = @Id
                );";


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
