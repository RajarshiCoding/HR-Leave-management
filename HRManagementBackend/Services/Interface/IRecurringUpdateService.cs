namespace HRManagementBackend.Services
{
    public interface IRecurringUpdateService
    {

        Task UpdateMonthlyAsync();

        Task UpdateYearlyAsync();
    }
}
