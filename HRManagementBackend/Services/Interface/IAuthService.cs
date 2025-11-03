using HRManagementBackend.Models;
using System.Threading.Tasks;

namespace HRManagementBackend.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string password);
        Task<EmployeeBasicInfo?> GetNameAsync(string email);
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<Employee?> GetEmployee(int EmpId);
        Task<int> VerifyAndChangePassword(Employee userData, string OldPassword, string NewPassword);
    }
}
