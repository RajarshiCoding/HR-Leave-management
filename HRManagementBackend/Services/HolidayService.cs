using Dapper;
using HRManagementBackend.Data;
using HRManagementBackend.Models;
using System.Data;

namespace HRManagementBackend.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly DapperContext _context;

        public HolidayService(DapperContext context)
        {
            _context = context;
        }

        // Get all holidays
        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
        {
            try
            {
                var query = @"SELECT * FROM holidays ORDER BY ""Date""";
                using var connection = _context.CreateConnection();
                return await connection.QueryAsync<Holiday>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch holidays. Please try again later.", ex);
            }
        }

        //Get holiday by date
        public async Task<Holiday?> GetHolidaysByDateAsync(DateTime date)
        {
            try
            {
                var query = @"
                    SELECT ""HolidayId"", ""Title"", ""Description"", ""Date"", ""CreatedAt""
                    FROM holidays
                    WHERE ""Date"" = @Date
                    LIMIT 1;
                ";
                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Holiday>(query, new { Date = date });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch holiday by date. Please try again later.", ex);
            }
        }

        // Add new holiday
        public async Task<int> AddHolidayAsync(Holiday holiday)
        {
            try
            {
                var query = @"
                    INSERT INTO holidays 
                    (""Title"", ""Description"", ""Date"", ""CreatedAt"")
                    VALUES 
                    (@Title, @Description, @Date, @CreatedAt)
                    RETURNING ""HolidayId"";
                ";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(query, holiday);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add holiday. Please try again later.", ex);
            }
        }

        // Update holiday
        public async Task<bool> UpdateHolidayAsync(Holiday holiday)
        {
            try
            {
                var query = @"
                    UPDATE holidays SET 
                    ""Title"" = @Title, 
                    ""Description"" = @Description, 
                    ""Date"" = @Date
                    WHERE 
                    ""HolidayId"" = @HolidayId;
                ";

                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, holiday);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update holiday. Please try again later.", ex);
            }
        }

        // Delete holiday
        public async Task<bool> DeleteHolidayAsync(DateTime date)
        {
            try
            {
                var query = @"DELETE FROM holidays WHERE ""Date"" = @Date";
                using var connection = _context.CreateConnection();
                var affectedRows = await connection.ExecuteAsync(query, new { Date = date });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to delete holiday. Please try again later.", ex);
            }
        }
    }
}
