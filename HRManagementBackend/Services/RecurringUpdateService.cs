using Dapper;
using HRManagementBackend.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RecurringUpdateService : BackgroundService
{
    private readonly DapperContext _context;
    private readonly ILogger<RecurringUpdateService> _logger;

    public RecurringUpdateService(DapperContext context, ILogger<RecurringUpdateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DateTime now = DateTime.Now;

                // Monthly update — 1st day of month
                if (now.Day == 1)
                    await UpdateMonthlyAsync();

                // Annual update — 1st January
                if (now.Day == 1 && now.Month == 1)
                    await UpdateYearlyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running recurring update tasks");
            }

            // Run once a day
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task UpdateMonthlyAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();

            var query = @"UPDATE employees
                          SET ""LeaveBalance"" = ""LeaveBalance"" + 2;";

            int rows = await connection.ExecuteAsync(query);
            _logger.LogInformation("Monthly update done successfully. {Count} rows affected.", rows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during monthly update.");
        }
    }

    private async Task UpdateYearlyAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();

            var query = @"
                UPDATE employees
                SET 
                    ""LeaveTaken"" = 0,
                    ""LeaveBalance"" = CASE 
                        WHEN ""LeaveBalance"" > 7 THEN 7 
                        ELSE ""LeaveBalance"" 
                    END;";

            int rows = await connection.ExecuteAsync(query);
            _logger.LogInformation("Annual update done successfully. {Count} rows affected.", rows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during annual update.");
        }
    }
}
