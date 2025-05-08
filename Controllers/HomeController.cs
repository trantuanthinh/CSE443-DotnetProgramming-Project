using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

namespace Project.Controllers
{
    public class HomeController(
        ILogger<HomeController> logger,
        IItemService itemService,
        IBorrowTransactionService borrowTransactionService
    ) : BaseController(logger: logger)
    {
        private readonly IItemService _itemService = itemService;
        private readonly IBorrowTransactionService _borrowTransactionService =
            borrowTransactionService;

        public async Task<IActionResult> Index()
        {
            var items = await _itemService.GetItems();
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var items = await _itemService.GetItems();

            var filtered = string.IsNullOrWhiteSpace(query)
                ? items
                : items
                    .Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return PartialView("_ItemList", filtered);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CurrentUser");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowRequest(
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
            var item = new BorrowTransaction
            {
                Id = Guid.NewGuid(),
                ItemId = itemId,
                Quantity = quantity,
                Status = ItemStatus.Pending,
                RequestDate = requestDate,
                BorrowerId = id,
            };
            bool isCreated = await _borrowTransactionService.CreateItem(item);
            if (isCreated)
            {
                _logger.LogInformation("Borrow request created");
            }
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
