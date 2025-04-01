using Microsoft.AspNetCore.Mvc;
using CalendarApi.Data;
using CalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedHolidaysController : ControllerBase
    {
        private readonly CalendarDbContext _context;

        public SavedHolidaysController(CalendarDbContext context)
        {
            _context = context;
        }

        // POST: api/SavedHolidays
        [HttpPost]
        public async Task<IActionResult> SaveHolidays([FromBody] SavedHoliday[] holidays)
        {
            if (holidays == null || !holidays.Any())
            {
                return BadRequest("No holidays provided to save.");
            }

            try
            {
                // Optionally clear existing saved holidays (remove this if you want to keep history)
                var existing = await _context.SavedHolidays.ToListAsync();
                _context.SavedHolidays.RemoveRange(existing);

                // Add new selections
                _context.SavedHolidays.AddRange(holidays);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Successfully saved {holidays.Length} holidays" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving holidays: {ex.Message}" });
            }
        }

        // GET: api/SavedHolidays
        [HttpGet]
        public async Task<ActionResult<SavedHoliday[]>> GetSavedHolidays()
        {
            var savedHolidays = await _context.SavedHolidays
                .OrderBy(sh => DateTime.Parse(sh.Date))
                .ToArrayAsync();
            return Ok(savedHolidays);
        }
    }
}