using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRManagementBackend.Models;
using HRManagementBackend.Services;

namespace HRManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HR")]
    public class VarsController : ControllerBase
    {
        private readonly CustomVarService _service;

        public VarsController(CustomVarService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vars = await _service.GetAllAsync();
            return Ok(vars);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVars([FromBody] List<CustomVar> vars)
        {
            if (vars == null || vars.Count == 0)
                return BadRequest(new { message = "No data provided" });

            var affected = await _service.UpsertAsync(vars);
            return Ok(new { message = $"Updated {affected} records" });
        }
    }
}
