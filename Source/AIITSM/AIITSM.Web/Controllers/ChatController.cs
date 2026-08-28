using AIITSM.Application._06_M6_AI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] List<ChatMessageDto> history)
        {
            if (history == null || history.Count == 0)
            {
                return BadRequest("No message provided.");
            }

            var reply = await _chatService.SendMessageAsync(history);
            return Json(new { reply });
        }
    }
}
