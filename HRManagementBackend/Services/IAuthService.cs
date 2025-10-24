using System.Threading.Tasks;
using HRManagementBackend.Models;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterRequest request);
}
