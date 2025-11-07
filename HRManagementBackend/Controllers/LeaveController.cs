using HRManagementBackend.Models;
using HRManagementBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        // GET: api/leave
        [Authorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> GetAllLeaves()
        {
            try
            {
                var leaves = await _leaveService.GetAllLeavesAsync();
                return Ok(leaves);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/leave/{requestId}
        [Authorize(Roles = "HR")]
        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetLeaveById(int requestId)
        {
            try
            {
                var leave = await _leaveService.GetLeaveByIdAsync(requestId);
                if (leave == null)
                    return NotFound(new { message = "Leave request not found" });
                return Ok(leave);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/leave/employee/{empId}
        [Authorize]
        [HttpGet("employee/{empId}")]
        public async Task<IActionResult> GetLeavesByEmployee(int empId)
        {
            try
            {
                var leaves = await _leaveService.GetLeavesByEmployeeIdAsync(empId);
                return Ok(leaves);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // POST: api/leave
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddLeave([FromBody] LeaveRequest leave)
        {
            try
            {
                if (leave == null)
                    return BadRequest(new { message = "Invalid leave request data" });

                // Set default values
                leave.Status = "Pending";
                leave.AppliedOn = DateTime.UtcNow;
                var result = await _leaveService.AddLeaveAsync(leave);

                if (result > 0)
                    return CreatedAtAction(nameof(GetLeaveById), new { requestId = result }, new { requestId = result });

                else if (result == 0)
                    return NoContent(); 

                else if (result == -1)
                    return BadRequest(new { message = "Failed to add leave request. Invalid data or duplicate entry." });

                else
                    return StatusCode(500, new { message = "Unexpected result from AddLeaveAsync." });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        // PUT: api/leave/{requestId}
        [Authorize(Roles = "HR")]
        [HttpPut("{requestId}")]
        public async Task<IActionResult> UpdateLeaveStatus(int requestId, [FromBody] LeaveStatusUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Invalid status data" });

                var leave = await _leaveService.GetLeaveByIdAsync(requestId);
                if (leave == null)
                    return NotFound(new { message = "Leave request not found" });

                if (dto.Status != null) leave.Status = dto.Status;
                if (dto.HrNote != null) leave.HrNote = dto.HrNote;

                var updated = await _leaveService.UpdateLeaveStatusAsync(leave);
                if (!updated)
                    return NotFound(new { message = "Leave request not found" });

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
        
        // GET: api/leave/update/{requestId}
        [Authorize(Roles = "HR")]
        [HttpPut("update/{requestId}")]
        public async Task<IActionResult> UpdateLeaveCounter(int requestId, [FromBody] int days)
        {
            try
            {
                var updated = await _leaveService.UpdateLeaveCounterAsync(requestId, days);
                if (!updated)
                    return NotFound(new { message = "Cannot update!!" });
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/leave/isAny
        [Authorize(Roles = "HR")]
        [HttpGet("isAny")]
        public async Task<IActionResult> HasPendingLeaves()
        {
            try
            {
                var hasPending = await _leaveService.HasPendingRequestsAsync();
                return Ok(new { hasPending });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
