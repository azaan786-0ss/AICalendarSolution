using AICalendar.Data;
using AICalendar.Domain.Entities;
using AICalendar.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AICalendar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/attendees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendeeDto>>> GetAttendees()
        {
            var attendees = await _context.Attendees.ToListAsync();
            var dtos = attendees.Select(a => new AttendeeDto
            {
                Id = a.Id,
                EventId = a.EventId,
                Email = a.Email,
                Name = a.Name,
                Status = a.Status
            });
            return Ok(dtos);
        }

        // GET: api/attendees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendeeDto>> GetAttendee(int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);

            if (attendee == null)
                return NotFound();

            var dto = new AttendeeDto
            {
                Id = attendee.Id,
                EventId = attendee.EventId,
                Email = attendee.Email,
                Name = attendee.Name,
                Status = attendee.Status
            };
            return Ok(dto);
        }

        // POST: api/attendees
        [HttpPost]
        public async Task<ActionResult<AttendeeDto>> CreateAttendee([FromBody] AttendeeDto attendeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check if parent event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == attendeeDto.EventId);
            if (!eventExists)
                return BadRequest("Invalid EventId.");

            var attendee = new Attendee
            {
                EventId = attendeeDto.EventId,
                Email = attendeeDto.Email,
                Name = attendeeDto.Name,
                Status = attendeeDto.Status
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            attendeeDto.Id = attendee.Id;
            return CreatedAtAction(nameof(GetAttendee), new { id = attendee.Id }, attendeeDto);
        }

        // PUT: api/attendees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendee(int id, [FromBody] AttendeeDto attendeeDto)
        {
            if (id != attendeeDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
                return NotFound();

            attendee.EventId = attendeeDto.EventId;
            attendee.Email = attendeeDto.Email;
            attendee.Name = attendeeDto.Name;
            attendee.Status = attendeeDto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/attendees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendee(int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
                return NotFound();

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
