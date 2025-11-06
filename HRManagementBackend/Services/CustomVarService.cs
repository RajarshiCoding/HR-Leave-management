using Dapper;
using HRManagementBackend.Data; // ✅ your existing context
using HRManagementBackend.Models;

namespace HRManagementBackend.Services
{
    public class CustomVarService
    {
        private readonly DapperContext _context;

        public CustomVarService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomVar>> GetAllAsync()
        {
            var query = @"SELECT ""varName"", ""value"" FROM customVar;";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CustomVar>(query);
        }

        public async Task<int> UpsertAsync(IEnumerable<CustomVar> vars)
        {
            var query = @"
                INSERT INTO customVar (""varName"", ""value"")
                VALUES (@VarName, @Value)
                ON CONFLICT (""varName"")
                DO UPDATE SET ""value"" = EXCLUDED.""value"";";

            using var connection = _context.CreateConnection();
            int total = 0;
            foreach (var v in vars)
                total += await connection.ExecuteAsync(query, v);

            return total;
        }
    }
}
