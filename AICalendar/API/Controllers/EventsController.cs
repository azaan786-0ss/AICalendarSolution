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
    public class EventController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            var dtos = events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CalendarId = e.CalendarId
            });
            return Ok(dtos);
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var evnt = await _context.Events.FindAsync(id);

            if (evnt == null)
                return NotFound();

            var dto = new EventDto
            {
                Id = evnt.Id,
                Title = evnt.Title,
                StartTime = evnt.StartTime,
                EndTime = evnt.EndTime,
                CalendarId = evnt.CalendarId
            };
            return Ok(dto);
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent(EventDto eventDto)
        {
            // check if parent calendar exists
            var calendarExists = await _context.Calendars.AnyAsync(c => c.Id == eventDto.CalendarId);
            if (!calendarExists)
                return BadRequest("Invalid CalendarId.");

            var evnt = new Event
            {
                Title = eventDto.Title,
                StartTime = eventDto.StartTime,
                EndTime = eventDto.EndTime,
                CalendarId = eventDto.CalendarId
            };

            _context.Events.Add(evnt);
            await _context.SaveChangesAsync();

            eventDto.Id = evnt.Id;
            return CreatedAtAction(nameof(GetEvent), new { id = evnt.Id }, eventDto);
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventDto eventDto)
        {
            if (id != eventDto.Id)
                return BadRequest();

            var evnt = await _context.Events.FindAsync(id);
            if (evnt == null)
                return NotFound();

            evnt.Title = eventDto.Title;
            evnt.StartTime = eventDto.StartTime;
            evnt.EndTime = eventDto.EndTime;
            evnt.CalendarId = eventDto.CalendarId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evnt = await _context.Events.FindAsync(id);
            if (evnt == null)
                return NotFound();

            _context.Events.Remove(evnt);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}