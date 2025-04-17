using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

namespace Project.Controllers
{
    public class ManagerController(
        ILogger<ManagerController> logger,
        IItemService itemService,
        IBorrowTransactionService borrowTransactionService
    ) : BaseController(logger: logger)
    {
        private readonly IItemService _itemService = itemService;
        private readonly IBorrowTransactionService _borrowTransactionService =
            borrowTransactionService;

        public async Task<IActionResult> Index()
        {
            var items = await _borrowTransactionService.GetItems();
            return View(items);
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
