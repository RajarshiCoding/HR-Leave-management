using HRManagementBackend.Models;
using HRManagementBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        // GET: api/holiday
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllHolidays()
        {
            try
            {
                var holidays = await _holidayService.GetAllHolidaysAsync();
                return Ok(holidays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/holiday/{date}
        [Authorize]
        [HttpGet("{date}")]
        public async Task<IActionResult> GetHolidayByDate(DateTime date)
        {
            try
            {
                var holidays = await _holidayService.GetHolidaysByDateAsync(date);
                if (holidays == null)
                    return NotFound();

                return Ok(holidays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // POST: api/holiday
        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> AddHoliday([FromBody] Holiday holiday)
        {
            try
            {
                if (holiday == null || string.IsNullOrEmpty(holiday.Title))
                    return BadRequest(new { message = "Invalid holiday data" });

                holiday.CreatedAt = DateTime.UtcNow;
                var id = await _holidayService.AddHolidayAsync(holiday);
                return CreatedAtAction(nameof(GetAllHolidays), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/holiday/{date}
        [Authorize(Roles = "HR")]
        [HttpPut("{date}")]
        public async Task<IActionResult> UpdateHoliday(DateTime date, [FromBody] HolidayUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Invalid holiday data" });

                var holidays = await _holidayService.GetHolidaysByDateAsync(date);
                if (holidays == null)
                    return NotFound();

                if (dto.Title != null) holidays.Title = dto.Title;
                if (dto.Description != null) holidays.Description = dto.Description;

                var updated = await _holidayService.UpdateHolidayAsync(holidays);
                if (!updated)
                    return NotFound(new { message = "Holiday not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // DELETE: api/holiday/{date}
        [Authorize(Roles = "HR")]
        [HttpDelete("{date}")]
        public async Task<IActionResult> DeleteHoliday(DateTime date)
        {
            try
            {
                var deleted = await _holidayService.DeleteHolidayAsync(date);
                if (!deleted)
                    return NotFound(new { message = "Holiday not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
