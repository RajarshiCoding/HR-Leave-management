using HRManagementBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRManagementBackend.Services
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveRequest>> GetAllLeavesAsync();
        Task<LeaveRequest?> GetLeaveByIdAsync(int requestId);
        Task<IEnumerable<LeaveRequest>> GetLeavesByEmployeeIdAsync(int empId);
        Task<int> AddLeaveAsync(LeaveRequest leave);
        Task<bool> UpdateLeaveStatusAsync(LeaveRequest leave);
        Task<bool> UpdateLeaveCounterAsync(int requestId);
        Task<bool> HasPendingRequestsAsync();
    }
}
