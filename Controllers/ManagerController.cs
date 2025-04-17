using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;
using Project.MailServices;
using Project.Models;
using Project.Utils;

namespace Project.Controllers
{
    public class ManagerController(
        ILogger<ManagerController> logger,
        MailService mailService,
        IItemService itemService,
        IBorrowTransactionService borrowTransactionService
    ) : BaseController(logger: logger)
    {
        private readonly MailService _mailService = mailService;
        private readonly IItemService _itemService = itemService;
        private readonly IBorrowTransactionService _borrowTransactionService =
            borrowTransactionService;

        public async Task<IActionResult> Index()
        {
            var items = await _borrowTransactionService.GetItems();
            var filterList = items.Where(item => item.Status == ItemStatus.Pending).ToList();
            return View(filterList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowResponse(Guid itemId, ItemStatus status)
        {
            _logger.LogInformation(
                "Borrow Response: Item ID = {0}, Status = {1}",
                itemId,
                status.ToString()
            );
            if (CurrentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var id = CurrentUser.Id;
            var item = await _borrowTransactionService.GetItem(itemId);
            item.ManagerId = id;
            item.Status = status;
            bool isUpdated = await _borrowTransactionService.EditItem(item);
            if (isUpdated)
            {
                _logger.LogInformation("Borrow Response Updated");
            }

            var body = _borrowTransactionService.GenerateBorrowResponseBody(
                CurrentUser.Name,
                item.Quantity,
                item.Status.ToString(),
                item.RequestDate
            );
            bool isSended = await _mailService.SendMail(
                CurrentUser.Email,
                "Borrow Response Status",
                body
            );
            if (isSended)
            {
                _logger.LogInformation("Mail Sended");
            }

            return RedirectToAction("Index", "Manager");
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
