using HRManagementBackend.Models;
using HRManagementBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Authenticate user and generate JWT token.
        /// </summary>
        /// <param name="dto">Login credentials (Email, Password)</param>
        /// <returns>JWT token if successful</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { message = "Email and password are required" });

            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new
            {
                message = "Login successful",
                token
            });
        }

        /// <summary>
        /// Register a new employee user.
        /// </summary>
        /// <param name="request">User registration data</param>
        /// <returns>Success message</returns>
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
    }
}
