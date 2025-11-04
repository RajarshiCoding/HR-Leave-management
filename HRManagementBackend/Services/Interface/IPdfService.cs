using HRManagementBackend.Models;

namespace HRManagementBackend.Services
{
    public interface IPdfService
    {
        byte[] GenerateEmployeeReport(Employee employee, IEnumerable<LeaveRequest> leaves);
    }
}
