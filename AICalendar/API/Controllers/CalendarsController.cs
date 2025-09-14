using AICalendar.Data;
using AICalendar.Domain.Entities;
using AICalendar.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AICalendar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CalendarController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/calendars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarDto>>> GetCalendars()
        {
            var calendars = await _context.Calendars.ToListAsync();
            var dtos = calendars.Select(c => new CalendarDto
            {
                Id = c.Id,
                Name = c.Name
            });
            return Ok(dtos);
        }

        // GET: api/calendars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CalendarDto>> GetCalendar(int id)
        {
            var calendar = await _context.Calendars.FindAsync(id);

            if (calendar == null)
                return NotFound();

            var dto = new CalendarDto
            {
                Id = calendar.Id,
                Name = calendar.Name
            };
            return Ok(dto);
        }

        // POST: api/calendars
        [HttpPost]
        public async Task<ActionResult<CalendarDto>> CreateCalendar(CalendarDto calendarDto)
        {
            var calendar = new Calendar
            {
                Name = calendarDto.Name
            };

            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();

            calendarDto.Id = calendar.Id;
            return CreatedAtAction(nameof(GetCalendar), new { id = calendar.Id }, calendarDto);
        }

        // PUT: api/calendars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCalendar(int id, CalendarDto calendarDto)
        {
            if (id != calendarDto.Id)
                return BadRequest();

            var calendar = await _context.Calendars.FindAsync(id);
            if (calendar == null)
                return NotFound();

            calendar.Name = calendarDto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/calendars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalendar(int id)
        {
            var calendar = await _context.Calendars.FindAsync(id);
            if (calendar == null)
                return NotFound();

            _context.Calendars.Remove(calendar);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
