public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class EmployeeBasicInfo
{
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string EmpId { get; set; } = string.Empty;
}
