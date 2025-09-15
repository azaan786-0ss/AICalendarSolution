

namespace AICalendar.API.Services
{
    public class IntentClassificationService
    {
        public async Task<string> ClassifyIntentAsync(string userInput, IChatCompletionService chatService)
        {
            var prompt = $"Classify the user's intent (create, update, cancel, list) for this input: \"{userInput}\". Respond with only the intent.";
            var result = await chatService.GetChatCompletionsAsync(prompt);
            return result.ToString().Trim().ToLower();
        }
    }
}
