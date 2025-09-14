using AICalendar.Data;
using AICalendar.Domain.Entities;
using AICalendar.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AICalendar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReminderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reminders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReminderDto>>> GetReminders()
        {
            var reminders = await _context.Reminders.ToListAsync();
            var dtos = reminders.Select(r => new ReminderDto
            {
                Id = r.Id,
                EventId = r.EventId,
                ReminderTime = r.ReminderTime
            });
            return Ok(dtos);
        }

        // GET: api/reminders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReminderDto>> GetReminder(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);

            if (reminder == null)
                return NotFound();

            var dto = new ReminderDto
            {
                Id = reminder.Id,
                EventId = reminder.EventId,
                ReminderTime = reminder.ReminderTime
            };
            return Ok(dto);
        }

        // POST: api/reminders
        [HttpPost]
        public async Task<ActionResult<ReminderDto>> CreateReminder(ReminderDto reminderDto)
        {
            // check if parent event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == reminderDto.EventId);
            if (!eventExists)
                return BadRequest("Invalid EventId.");

            var reminder = new Reminder
            {
                EventId = reminderDto.EventId,
                ReminderTime = reminderDto.ReminderTime
            };

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            reminderDto.Id = reminder.Id;
            return CreatedAtAction(nameof(GetReminder), new { id = reminder.Id }, reminderDto);
        }

        // PUT: api/reminders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, ReminderDto reminderDto)
        {
            if (id != reminderDto.Id)
                return BadRequest();

            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
                return NotFound();

            reminder.EventId = reminderDto.EventId;
            reminder.ReminderTime = reminderDto.ReminderTime;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/reminders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
                return NotFound();

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
