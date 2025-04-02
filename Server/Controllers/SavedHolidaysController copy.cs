using Microsoft.AspNetCore.Mvc;
using CalendarApi.Data;
using CalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                foreach (var holiday in holidays)
                {
                    bool exists = await _context.SavedHolidays.AnyAsync(h => h.Date == holiday.Date);
                    if (!exists)
                    {
                        _context.SavedHolidays.Add(holiday);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Successfully saved {holidays.Length} holidays" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving holidays: {ex.Message}");
                return StatusCode(500, new { message = $"Error saving holidays: {ex.Message}" });
            }
        }

        // GET: api/SavedHolidays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavedHoliday>>> GetSavedHolidays()
        {
            var savedHolidays = await _context.SavedHolidays.ToListAsync();

            var sortedHolidays = savedHolidays
                .Where(sh => DateTime.TryParse(sh.Date, out _)) // Ensure valid dates
                .OrderBy(sh => DateTime.Parse(sh.Date)) // Perform sorting in-memory
                .ToList();

            return Ok(sortedHolidays);
        }

        // DELETE: api/SavedHolidays/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            var holiday = await _context.SavedHolidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound(new { message = "Holiday not found" });
            }

            _context.SavedHolidays.Remove(holiday);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Holiday deleted successfully" });
        }

        // PUT: api/SavedHolidays/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHoliday(int id, [FromBody] SavedHoliday updatedHoliday)
        {
            if (id != updatedHoliday.Id)
            {
                return BadRequest("Mismatched holiday ID");
            }

            var holiday = await _context.SavedHolidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound(new { message = "Holiday not found" });
            }

            holiday.Date = updatedHoliday.Date;
            holiday.EventName = updatedHoliday.EventName;

            _context.SavedHolidays.Update(holiday);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Holiday updated successfully" });
        }
    }
}
