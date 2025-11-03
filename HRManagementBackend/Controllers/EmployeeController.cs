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
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employee
        [Authorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });
            return Ok(employee);
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest(new { message = "Invalid employee data" });

            var empId = await _employeeService.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = empId }, new { empId });
        }

        // This was the past PUT command changed to new PUT DTO, let's see !!
        // PUT: api/employee/{id}
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        // {
        //     if (employee == null || id != employee.EmpId)
        //         return BadRequest(new { message = "Invalid employee data" });

        //     var updated = await _employeeService.UpdateEmployeeAsync(employee);
        //     if (!updated)
        //         return NotFound(new { message = "Employee not found" });

        //     return NoContent();
        // }

        //******************************************************************************

        // PUT: api/employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid employee data" });

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            // Update only fields provided
            if (dto.Name != null) employee.Name = dto.Name;
            if (dto.Email != null) employee.Email = dto.Email;
            if (dto.Department != null) employee.Department = dto.Department;
            if (dto.Designation != null) employee.Designation = dto.Designation;
            if (dto.Contact != null) employee.Contact = dto.Contact;
            if (dto.LeaveBalance.HasValue) employee.LeaveBalance = dto.LeaveBalance.Value;
            if (dto.LeaveTaken.HasValue) employee.LeaveTaken = dto.LeaveTaken.Value;
            // if (dto.Status != null) employee.Status = dto.Status;

            var updated = await _employeeService.UpdateEmployeeAsync(employee);
            if (!updated)
                return NotFound(new { message = "Employee not found" });

            return NoContent();
        }


        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);
            if (!deleted)
                return NotFound(new { message = "Employee not found" });

            return NoContent();
        }
    }
}
