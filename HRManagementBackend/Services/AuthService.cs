using HRManagementBackend.Data;
using HRManagementBackend.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace HRManagementBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _config;

        public AuthService(DapperContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            try
            {
                var query = @"SELECT * FROM employees WHERE ""Email"" = @Email";
                using var connection = _context.CreateConnection();
                var user = await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Email = email });

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return null;

                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to login. Please try again later.", ex);
            }
        }

        public async Task<EmployeeBasicInfo?> GetNameAsync(string email)
        {
            try
            {
                const string query = @"SELECT ""Name"", ""Designation"", ""EmpId""
                                       FROM employees 
                                       WHERE ""Email"" = @Email";

                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<EmployeeBasicInfo>(query, new { Email = email });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve employee information.", ex);
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var checkQuery = @"SELECT COUNT(*) FROM employees WHERE ""Email"" = @Email";
                var insertQuery = @"
                    INSERT INTO employees 
                    (""Name"", ""Email"", ""PasswordHash"", ""Department"", ""Designation"", ""Contact"")
                    VALUES (@Name, @Email, @PasswordHash, @Department, @Designation, @Contact)
                ";

                using var connection = _context.CreateConnection();

                var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { request.Email });
                if (exists > 0)
                    return false;

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                await connection.ExecuteAsync(insertQuery, new
                {
                    request.Name,
                    request.Email,
                    PasswordHash = passwordHash,
                    request.Department,
                    request.Designation,
                    request.Contact
                });

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to register user. Please try again later.", ex);
            }
        }

        public async Task<Employee?> GetEmployee(int EmpId)
        {
            try
            {
                const string query = @"SELECT * FROM employees WHERE ""EmpId"" = @EmpId";
                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { EmpId });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to fetch employee details.", ex);
            }
        }

        public async Task<int> VerifyAndChangePassword(Employee userData, string OldPassword, string NewPassword)
        {
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(OldPassword, userData.PasswordHash))
                    return 0;

                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

                var query = @"
                    UPDATE employees 
                    SET ""PasswordHash"" = @NewPasswordHash
                    WHERE ""EmpId"" = @EmpId;
                ";

                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    NewPasswordHash = newPasswordHash,
                    EmpId = userData.EmpId
                });
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to change password. Please try again later.", ex);
            }
        }

        private string GenerateJwtToken(Employee user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("empId", user.EmpId.ToString()),
                new Claim(ClaimTypes.Role, user.Designation)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
