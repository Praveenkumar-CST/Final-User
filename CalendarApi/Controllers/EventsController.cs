using Microsoft.AspNetCore.Mvc;
using CalendarApi.Data;
using CalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly CalendarDbContext _context;

        public EventsController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/events/2025-03-31
        [HttpGet("{date}")]
        public async Task<ActionResult<Event>> GetEvent(string date)
        {
            var evt = await _context.Events.FirstOrDefaultAsync(e => e.Date == date);
            if (evt == null) return NotFound();
            return evt;
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event evt)
        {
            if (evt == null || string.IsNullOrEmpty(evt.Date) || string.IsNullOrEmpty(evt.HolidayType))
            {
                return BadRequest("Date and HolidayType are required.");
            }

            // Automatically set the Day field
            if (DateTime.TryParseExact(evt.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                evt.Day = parsedDate.DayOfWeek.ToString();
            }
            
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEvent), new { date = evt.Date }, evt);
        }

        // DELETE: api/events/2025-03-31
        [HttpDelete("{date}")]
        public async Task<IActionResult> DeleteEvent(string date)
        {
            var evt = await _context.Events.FirstOrDefaultAsync(e => e.Date == date);
            if (evt == null) return NotFound();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
