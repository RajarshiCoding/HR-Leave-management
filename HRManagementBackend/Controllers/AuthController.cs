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
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { message = "Email and password are required" });

            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            var empInfo = await _authService.GetNameAsync(dto.Email);

            Console.WriteLine(empInfo.EmpId);
            
            if (empInfo == null)
                return NotFound(new { message = "User not found" });
            
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new
            {
                message = "Login successful",
                token,
                empInfo.Name,
                empInfo.Designation,
                empInfo.EmpId
            });
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Invalid registration data" });

            var result = await _authService.RegisterAsync(request);

            if (!result)
                return BadRequest(new { message = "Email already exists" });

            return Ok(new { message = "User registered successfully" });
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Authorize()
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
    }
}
