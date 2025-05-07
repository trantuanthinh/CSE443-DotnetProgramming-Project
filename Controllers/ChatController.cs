using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

namespace Project.Controllers
{
    public class ChatController(
        ILogger<ChatController> logger,
        IConversationService conservationService,
        IMessageService messageService
    ) : BaseController(logger: logger)
    {
        private readonly IConversationService _conservationService = conservationService;
        private readonly IMessageService _messageService = messageService;

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(
            Guid itemId,
            int quantity,
            DateTime requestDate
        )
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (requestDate <= DateTime.Now)
            {
                _logger.LogInformation("Invalid Date");
                return RedirectToAction("Index", "Home");
            }

            var id = CurrentUser.Id;

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
