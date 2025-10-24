namespace HRManagementBackend.Models
{
    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = "Employee"; // or HR/Admin
        public string Contact { get; set; } = string.Empty;
    }
}
