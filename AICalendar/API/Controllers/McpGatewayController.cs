using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AICalendar.API.Services;
using AICalendar.Data;
using AICalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/mcp-gateway")]
public class McpGatewayController : ControllerBase
{
    private readonly IntentClassificationService _intentService;
    private readonly IChatCompletionService _chatService;
    private readonly AppDbContext _db;
    private readonly ILogger<McpGatewayController> _logger;
    private readonly bool _redactTitle;

    public McpGatewayController(
        IntentClassificationService intentService,
        IChatCompletionService chatService,
        AppDbContext db,
        ILogger<McpGatewayController> logger,
        IConfiguration config)
    {
        _intentService = intentService;
        _chatService = chatService;
        _db = db;
        _logger = logger;
        _redactTitle = config.GetValue<bool>("RedactTitle");
    }

    [HttpPost("route")]
    public async Task<IActionResult> RouteUserInput([FromBody] JsonObject payload)
    {
        var userInput = payload["user_input"]?.ToString();
        if (string.IsNullOrWhiteSpace(userInput))
            return BadRequest(new { ok = false, error = "Missing user_input" });

        // 1. Classify intent using LLM
        var intent = await _intentService.ClassifyIntentAsync(userInput, _chatService);

        // Log LLM output
        _logger.LogInformation("LLM output: {Intent}", intent);

        // 2. Route to MCP tool based on intent
        switch (intent)
        {
            case "create":
                return await SaveEvent(payload);
            case "update":
                return await UpdateEvent(payload);
            case "cancel":
                return await CancelEvent(payload);
            case "list":
                return await ListEvents(payload);
            default:
                return BadRequest(new { ok = false, error = "Unknown intent" });
        }
    }

    // MCP tool: save_event
    private async Task<IActionResult> SaveEvent(JsonObject payload)
    {
        var clientRefId = payload["client_reference_id"]?.ToString();
        var title = payload["title"]?.ToString()?.Trim();
        var startStr = payload["start"]?.ToString();
        var endStr = payload["end"]?.ToString();

        var missingFields = new List<string>();
        if (string.IsNullOrWhiteSpace(clientRefId)) missingFields.Add("client_reference_id");
        if (string.IsNullOrWhiteSpace(title)) missingFields.Add("title");
        if (string.IsNullOrWhiteSpace(startStr)) missingFields.Add("start");
        if (string.IsNullOrWhiteSpace(endStr)) missingFields.Add("end");

        if (missingFields.Count > 0)
            return BadRequest(new { ok = false, error = "missing_fields", fields = missingFields });

        var start = DateTime.Parse(startStr);
        var end = DateTime.Parse(endStr);
        var timezone = payload["timezone"]?.ToString();
        var location = payload["location"]?.ToString();
        var attendees = payload["attendees"]?.ToString();
        var notes = payload["notes"]?.ToString();

        if (end <= start)
            return BadRequest(new { ok = false, error = "end must be after start" });

        // Clamp/trim fields as needed
        title = title?.Length > 100 ? title.Substring(0, 100) : title;

        // Idempotency: Upsert by client_reference_id
        var existing = await _db.Events.FirstOrDefaultAsync(e => e.ClientReferenceId == clientRefId);
        if (existing != null)
        {
            // No-op update
            return Ok(new
            {
                ok = true,
                data = new { id = existing.Id, existing.Title, existing.StartTime, existing.EndTime, existing.Timezone }
            });
        }
        else
        {
            var newEvent = new Event
            {
                Title = title,
                StartTime = start,
                EndTime = end,
                Timezone = timezone,
                Location = location,
                AttendeesRaw = attendees,
                Notes = notes,
                ClientReferenceId = clientRefId
            };
            _db.Events.Add(newEvent);
            await _db.SaveChangesAsync();

            // Log MCP call
            _logger.LogInformation("MCP call: {Method}", "create");

            // Log DB id and title (sensitive data redaction)
            _logger.LogInformation("DB id: {Id}, title: {Title}", newEvent.Id, _redactTitle ? "[REDACTED]" : newEvent.Title);

            return Ok(new
            {
                ok = true,
                data = new { id = newEvent.Id, newEvent.Title, newEvent.StartTime, newEvent.EndTime, newEvent.Timezone }
            });
        }
    }

    // MCP tool: update_event
    private async Task<IActionResult> UpdateEvent(JsonObject payload)
    {
        var id = payload["id"]?.ToString();
        var clientRefId = payload["client_reference_id"]?.ToString();

        var existing = await _db.Events.FirstOrDefaultAsync(e => e.Id.ToString() == id || e.ClientReferenceId == clientRefId);
        if (existing == null)
            return NotFound(new { ok = false, error = "event not found" });

        if (payload.ContainsKey("title")) existing.Title = payload["title"]?.ToString();
        if (payload.ContainsKey("start")) existing.StartTime = DateTime.Parse(payload["start"]?.ToString() ?? "");
        if (payload.ContainsKey("end")) existing.EndTime = DateTime.Parse(payload["end"]?.ToString() ?? "");
        if (payload.ContainsKey("timezone")) existing.Timezone = payload["timezone"]?.ToString();
        if (payload.ContainsKey("location")) existing.Location = payload["location"]?.ToString();
        if (payload.ContainsKey("attendees") && payload["attendees"] is JsonArray attendeesArray)
        {
            existing.Attendees = attendeesArray
                .Select(a => new Attendee
                {
                    Name = a?["name"]?.ToString(),
                    Email = a?["email"]?.ToString(),
                    Status = a?["status"]?.ToString()
                })
                .ToList();
        }
        if (payload.ContainsKey("notes")) existing.Notes = payload["notes"]?.ToString();

        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    // MCP tool: cancel_event
    private async Task<IActionResult> CancelEvent(JsonObject payload)
    {
        var id = payload["id"]?.ToString();
        var clientRefId = payload["client_reference_id"]?.ToString();

        var existing = await _db.Events.FirstOrDefaultAsync(e => e.Id.ToString() == id || e.ClientReferenceId == clientRefId);
        if (existing == null)
            return NotFound(new { ok = false, error = "event not found" });

        _db.Events.Remove(existing);
        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    // MCP tool: list_events (optional, basic implementation)
    private async Task<IActionResult> ListEvents(JsonObject payload)
    {
        var events = await _db.Events.ToListAsync();
        var data = events.Select(e => new
        {
            id = e.Id,
            title = e.Title,
            start = e.StartTime,
            end = e.EndTime,
            timezone = e.Timezone
        });
        return Ok(new { ok = true, data });
    }
}