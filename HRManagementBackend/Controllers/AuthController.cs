using HRManagementBackend.Models;
using HRManagementBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                    return BadRequest(new { message = "Email and password are required" });

                var token = await _authService.LoginAsync(dto.Email, dto.Password);
                if (token == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var empInfo = await _authService.GetNameAsync(dto.Email);
                if (empInfo == null)
                    return NotFound(new { message = "User not found" });

                return Ok(new
                {
                    message = "Login successful",
                    token,
                    empInfo.Name,
                    empInfo.Designation,
                    empInfo.EmpId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return BadRequest(new { message = "Invalid registration data" });

                var result = await _authService.RegisterAsync(request);

                if (!result)
                    return BadRequest(new { message = "Email already exists" });

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var name = User.Identity?.Name;

                return Ok(new
                {
                    message = "Authorized",
                    role,
                    name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPut("changepass")]
        [Authorize]
        public async Task<IActionResult> ChangePass([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (dto == null || dto.EmpId <= 0 ||
                    string.IsNullOrEmpty(dto.OldPassword) || string.IsNullOrEmpty(dto.NewPassword))
                {
                    return BadRequest(new { message = "Employee ID, old password, and new password are required." });
                }

                var userData = await _authService.GetEmployee(dto.EmpId);
                if (userData == null)
                    return NotFound(new { message = "User not found." });

                var rowsAffected = await _authService.VerifyAndChangePassword(userData, dto.OldPassword, dto.NewPassword);

                if (rowsAffected == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update password. Check old password or database error." });

                return Ok(new
                {
                    message = "Password changed successfully!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
