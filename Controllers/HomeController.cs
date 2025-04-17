using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
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

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CurrentUser");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowRequest(Guid itemId, int quantity)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var id = CurrentUser.Id;
            _logger.LogInformation($"Borrow request: Item ID = {itemId}, Quantity = {quantity}");
            var item = new BorrowTransaction
            {
                Id = Guid.NewGuid(),
                ItemId = itemId,
                Quantity = quantity,
                Status = ItemStatus.Pending,
                RequestDate = DateTime.Now,
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
