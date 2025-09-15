using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AICalendar.API.Services;
using AICalendar.Data;
using AICalendar.Domain.Entities;

[ApiController]
[Route("api/mcp-gateway")]
public class McpGatewayController : ControllerBase
{
    private readonly IntentClassificationService _intentService;
    private readonly IChatCompletionService _chatService;
    private readonly AppDbContext _db;

    public McpGatewayController(
        IntentClassificationService intentService,
        IChatCompletionService chatService,
        AppDbContext db)
    {
        _intentService = intentService;
        _chatService = chatService;
        _db = db;
    }

    [HttpPost("route")]
    public async Task<IActionResult> RouteUserInput([FromBody] JsonObject payload)
    {
        var userInput = payload["user_input"]?.ToString();
        if (string.IsNullOrWhiteSpace(userInput))
            return BadRequest(new { ok = false, error = "Missing user_input" });

        // 1. Classify intent using LLM
        var intent = await _intentService.ClassifyIntentAsync(userInput, _chatService);

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
        var start = DateTime.Parse(payload["start"]?.ToString() ?? "");
        var end = DateTime.Parse(payload["end"]?.ToString() ?? "");
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
        if (payload.ContainsKey("attendees")) existing.Attendees = payload["attendees"]?.ToString();
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