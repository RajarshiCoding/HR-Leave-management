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
    public class AuthService
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
            var query = @"SELECT * FROM employees WHERE ""Email"" = @Email";
            using var connection = _context.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Email = email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }
        
        public async Task<EmployeeBasicInfo?> GetNameAsync(string email)
        {
            const string query = @"SELECT ""Name"", ""Designation"" , ""EmpId""
                                FROM employees 
                                WHERE ""Email"" = @Email";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<EmployeeBasicInfo>(query, new { Email = email });

            return result;
        }



        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var checkQuery = @"SELECT COUNT(*) FROM employees WHERE ""Email"" = @Email";
            // var insertQuery = @"
            //     INSERT INTO employees 
            //     (""Name"", ""Email"", ""PasswordHash"", ""PasswordSalt"", ""Department"",""Designation"",""Contact"", ""Status"")
            //     VALUES (@Name, @Email, @PasswordHash, @PasswordSalt, @Department, @Designation, @Contact, 'Active')
            // ";
            var insertQuery = @"
                INSERT INTO employees 
                (""Name"", ""Email"", ""PasswordHash"", ""Department"",""Designation"",""Contact"", ""Status"")
                VALUES (@Name, @Email, @PasswordHash, @Department, @Designation, @Contact, 'Active')
            ";

            using var connection = _context.CreateConnection();

            var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { request.Email });
            if (exists > 0)
                return false;

            // ✅ Generate password hash and salt
            // var salt = Guid.NewGuid().ToString("N"); // or a more secure random generator
            // var passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password + salt));

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
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
