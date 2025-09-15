public interface IChatCompletionService
{
    Task<object> GetChatCompletionsAsync(string prompt);
}