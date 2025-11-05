using HRManagementBackend.Models;
using HRManagementBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveService _leaveService;
        private readonly IPdfService _pdfService;

        public EmployeeController(IEmployeeService employeeService, ILeaveService leaveService, IPdfService pdfService)
        {
            _employeeService = employeeService;
            _leaveService = leaveService;
            _pdfService = pdfService;
        }

        // GET: api/employee
        [Authorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/employee/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // POST: api/employee
        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                    return BadRequest(new { message = "Invalid employee data" });

                var empId = await _employeeService.AddEmployeeAsync(employee);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = empId }, new { empId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/employee/{id}
        [Authorize(Roles = "HR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Invalid employee data" });

                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                if (dto.Name != null) employee.Name = dto.Name;
                if (dto.Email != null) employee.Email = dto.Email;
                if (dto.Department != null) employee.Department = dto.Department;
                if (dto.Designation != null) employee.Designation = dto.Designation;
                if (dto.Contact != null) employee.Contact = dto.Contact;
                if (dto.LeaveBalance.HasValue) employee.LeaveBalance = dto.LeaveBalance.Value;
                if (dto.LeaveTaken.HasValue) employee.LeaveTaken = dto.LeaveTaken.Value;

                var updated = await _employeeService.UpdateEmployeeAsync(employee);
                if (!updated)
                    return NotFound(new { message = "Employee not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // DELETE: api/employee/{id}
        [Authorize(Roles = "HR")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var deleted = await _employeeService.DeleteEmployeeAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Employee not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/employee/{id}/report
        [Authorize(Roles = "HR")]
        [HttpGet("{id}/report")]
        public async Task<IActionResult> GetEmployeeReport(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                var leaves = await _leaveService.GetLeavesByEmployeeIdAsync(id);
                var pdfBytes = _pdfService.GenerateEmployeeReport(employee, leaves);

                return File(pdfBytes, "application/pdf", "Report.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
