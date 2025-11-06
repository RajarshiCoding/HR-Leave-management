using System;
using System.Threading.Tasks;
using Dapper;
using HRManagementBackend.Data;

namespace HRManagementBackend.Services
{
    public class JobTracker : IJobTracker
    {
        private readonly DapperContext _context;

        public JobTracker(DapperContext context)
        {
            _context = context;
        }

        public async Task<DateTime?> GetLastRunAsync(string jobName)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT ""last_run"" FROM system_jobs WHERE ""job_name"" = @JobName;";
            return await connection.QuerySingleOrDefaultAsync<DateTime?>(sql, new { JobName = jobName });
        }

        public async Task UpdateLastRunAsync(string jobName, DateTime timestamp)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO system_jobs (""job_name"", ""last_run"")
                VALUES (@JobName, @Timestamp)
                ON CONFLICT (""job_name"")
                DO UPDATE SET ""last_run"" = @Timestamp;";
            await connection.ExecuteAsync(sql, new { JobName = jobName, Timestamp = timestamp });
        }
    }
}
