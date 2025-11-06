using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HRManagementBackend.Data;
using HRManagementBackend.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RecurringUpdateService : BackgroundService
{
    private readonly DapperContext _context;
    private readonly IJobTracker _jobTracker;
    private readonly ILogger<RecurringUpdateService> _logger;

    public RecurringUpdateService(
        DapperContext context,
        IJobTracker jobTracker,
        ILogger<RecurringUpdateService> logger)
    {
        _context = context;
        _jobTracker = jobTracker;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Run both checks every time the service wakes up.
                await RunJobIfDueAsync(
                    "MonthlyUpdate",
                    lastRun => IsMonthlyDue(lastRun, DateTime.Now),
                    UpdateMonthlyAsync);

                await RunJobIfDueAsync(
                    "YearlyUpdate",
                    lastRun => IsYearlyDue(lastRun, DateTime.Now),
                    UpdateYearlyAsync);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running recurring update tasks");
            }

            // Run once a day
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    // Common runner: check last run, run job, update DB
    private async Task RunJobIfDueAsync(string jobName, Func<DateTime?, bool> isDue, Func<Task> jobAction)
    {
        try
        {
            var lastRun = await _jobTracker.GetLastRunAsync(jobName);

            if (isDue(lastRun))
            {
                _logger.LogInformation("Running job {JobName} (last run: {LastRun})...", jobName, lastRun);
                await jobAction();
                await _jobTracker.UpdateLastRunAsync(jobName, DateTime.Now);
                _logger.LogInformation("Job {JobName} completed and timestamp updated.", jobName);
            }
            else
            {
                _logger.LogInformation("Job {JobName} not due yet (last run: {LastRun}).", jobName, lastRun);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing job {JobName}", jobName);
        }
    }

    // Due checks (UTC-based)
    private bool IsMonthlyDue(DateTime? lastRun, DateTime nowUtc)
    {
        // due when last run is null OR last run's year/month is earlier than current
        return lastRun == null || lastRun.Value.Year < nowUtc.Year || lastRun.Value.Month < nowUtc.Month;
    }

    private bool IsYearlyDue(DateTime? lastRun, DateTime nowUtc)
    {
        // due when last run is null OR last run's year is earlier than current
        return lastRun == null || lastRun.Value.Year < nowUtc.Year;
    }

    // Actual DB update: add 2 leave days monthly
    private async Task UpdateMonthlyAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();

            var getMonthlyLeaveQuery = @"SELECT ""MonthlyUpdateDay"" FROM customVar;";
            var updateQuery = @"UPDATE employees
                                SET ""LeaveBalance"" = ""LeaveBalance"" + @days;";

            int days = await connection.QueryFirstOrDefaultAsync<int>(getMonthlyLeaveQuery);

            if (days <= 0)
            {
                _logger.LogWarning("Monthly update skipped — invalid or missing MonthlyUpdateDay value: {Days}", days);
                return;
            }

            int rows = await connection.ExecuteAsync(updateQuery, new { days });
            _logger.LogInformation("Monthly update done. {Count} rows affected (added {Days} days).", rows, days);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during monthly update.");
        }

    }

    // Actual DB update: reset LeaveTaken and cap LeaveBalance yearly
    private async Task UpdateYearlyAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();

            //Fetch configurable yearly carry-forward limit (if any)
            var getCarryLimitQuery = @"SELECT ""MaxCarryForward"" FROM customVar;";
            int maxCarryForward = await connection.QueryFirstOrDefaultAsync<int>(getCarryLimitQuery);

            if (maxCarryForward <= 0)
            {
                // Default fallback if value is missing or invalid
                maxCarryForward = 7;
                _logger.LogWarning("No valid MaxCarryForward found in customVar. Using default value: {Value}", maxCarryForward);
            }

            //Reset LeaveTaken and cap LeaveBalance
            var updateQuery = @"
                UPDATE employees
                SET 
                    ""LeaveTaken"" = 0,
                    ""LeaveBalance"" = CASE 
                        WHEN ""LeaveBalance"" > @max THEN @max
                        ELSE ""LeaveBalance"" 
                    END;";

            int rows = await connection.ExecuteAsync(updateQuery, new { max = maxCarryForward });

            //Log result
            _logger.LogInformation(
                "Yearly update completed successfully. {Count} rows affected. Carry-forward limit: {Limit}",
                rows,
                maxCarryForward
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during annual update.");
            throw; // rethrow to ensure it’s logged at higher level if needed
        }
    }
}
