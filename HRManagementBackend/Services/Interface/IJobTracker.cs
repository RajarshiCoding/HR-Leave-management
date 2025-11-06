using System;
using System.Threading.Tasks;

namespace HRManagementBackend.Services
{
    public interface IJobTracker
    {
        Task<DateTime?> GetLastRunAsync(string jobName);
        Task UpdateLastRunAsync(string jobName, DateTime timestamp);
    }
}
