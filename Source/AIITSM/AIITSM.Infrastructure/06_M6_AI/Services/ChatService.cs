using System.Text;
using AIITSM.Application._06_M6_AI.Services;
using Google.GenAI;

namespace AIITSM.Infrastructure._06_M6_AI.Services
{
    public class ChatService : IChatService
    {
        private readonly Client _client;

        public ChatService()
        {
            _client = new Client();
        }

        public async Task<string> SendMessageAsync(List<ChatMessageDto> history)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are a helpful AI assistant for AI-ITSM, an IT service desk platform. Answer clearly and concisely.");
            sb.AppendLine();

            foreach (var msg in history)
            {
                var speaker = msg.Role == "user" ? "User" : "Assistant";
                sb.AppendLine($"{speaker}: {msg.Text}");
            }
            sb.AppendLine("Assistant:");

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.5-flash-lite",
                contents: sb.ToString());

            var text = response.Candidates?
                .FirstOrDefault()?
                .Content?
                .Parts?
                .FirstOrDefault()?
                .Text;

            return string.IsNullOrWhiteSpace(text)
                ? "Sorry, I couldn't generate a response."
                : text;
        }
    }
}