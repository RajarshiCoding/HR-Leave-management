using HRManagementBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRManagementBackend.Services
{
    public interface IHolidayService
    {
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync();
        Task<Holiday?> GetHolidaysByDateAsync(DateTime date);
        Task<int> AddHolidayAsync(Holiday holiday);
        Task<bool> UpdateHolidayAsync(Holiday holiday);
        Task<bool> DeleteHolidayAsync(DateTime date);
    }
}
