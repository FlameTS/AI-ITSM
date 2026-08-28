namespace AIITSM.Application._06_M6_AI.Services
{
    public class ChatMessageDto
    {
        public string Role { get; set; } = "user"; // "user" or "assistant"
        public string Text { get; set; } = string.Empty;
    }

    public interface IChatService
    {
        Task<string> SendMessageAsync(List<ChatMessageDto> history);
    }
}