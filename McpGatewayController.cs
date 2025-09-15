using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using AICalendar.API.Services;

[ApiController]
[Route("api/mcp-gateway")]
public class McpGatewayController : ControllerBase
{
    private readonly IntentClassificationService _intentService;
    private readonly IChatCompletionService _chatService;

    public McpGatewayController(IntentClassificationService intentService, IChatCompletionService chatService)
    {
        _intentService = intentService;
        _chatService = chatService;
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
                // Call your MCP save_event logic here
                // Example: return await SaveEvent(payload);
                return Ok(new { ok = true, data = "Would call save_event MCP tool" });
            case "update":
                // Call your MCP update_event logic here
                return Ok(new { ok = true, data = "Would call update_event MCP tool" });
            case "cancel":
                // Call your MCP cancel_event logic here
                return Ok(new { ok = true, data = "Would call cancel_event MCP tool" });
            case "list":
                // Call your MCP list_events logic here
                return Ok(new { ok = true, data = "Would call list_events MCP tool" });
            default:
                return BadRequest(new { ok = false, error = "Unknown intent" });
        }
    }
}