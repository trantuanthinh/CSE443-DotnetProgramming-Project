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

        public async Task<IActionResult> BorrowRequest()
        {
            var items = await _borrowTransactionService.GetItems();
            var filterList = items.Where(item => item.Status == ItemStatus.Pending).ToList();
            return View(filterList);
        }

        public async Task<IActionResult> BorrowingItemList()
        {
            var items = await _borrowTransactionService.GetItems();
            var filterList = items
                .Where(item =>
                    item.Status == ItemStatus.Approved || item.Status == ItemStatus.Borrowing
                )
                .ToList();
            return View(filterList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowResponse(Guid itemId, ItemStatus status)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            await using var transaction = await _borrowTransactionService.BeginTransactionAsync();
            try
            {
                var borrowTransaction = await _borrowTransactionService.GetItem(itemId);
                if (borrowTransaction == null)
                {
                    _logger.LogInformation("Borrow Transaction not found");
                    return RedirectToAction("Index", "Manager");
                }

                var item = await _itemService.GetItem(borrowTransaction.Item.Id);
                if (
                    item == null
                    || (status == ItemStatus.Approved && borrowTransaction.Quantity > item.Quantity)
                )
                {
                    _logger.LogInformation("Invalid Borrow Quantity");
                    return RedirectToAction("Index", "Manager");
                }

                borrowTransaction.ManagerId = CurrentUser.Id;
                borrowTransaction.DueDate = borrowTransaction.RequestDate.AddDays(7);
                borrowTransaction.Status = status;
                if (!await _borrowTransactionService.EditItem(borrowTransaction))
                {
                    throw new Exception("Failed to update Borrow Transaction");
                }

                item.Quantity -= borrowTransaction.Quantity;
                if (!await _itemService.EditItem(item))
                {
                    throw new Exception("Failed to update Item");
                }

                // Open For Testing Email/ Demo
                // var body = _borrowTransactionService.GenerateBorrowResponseBody(
                //     CurrentUser.Name,
                //     borrowTransaction.Quantity,
                //     borrowTransaction.Status.ToString(),
                //     borrowTransaction.RequestDate
                // );
                // if (!await _mailService.SendMail(CurrentUser.Email, "Borrow Response Status", body))
                // {
                //     throw new Exception("Failed to send email");
                // }

                await transaction.CommitAsync();
                _logger.LogInformation("Transaction Success");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Transaction Failed: {0}", ex.Message);
            }
            return RedirectToAction("BorrowRequest", "Manager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageBorrowItem(Guid itemId, ItemStatus status)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Open For Testing Email/ Demo
            // var body = _borrowTransactionService.GenerateBorrowResponseBody(
            //     CurrentUser.Name,
            //     borrowTransaction.Quantity,
            //     borrowTransaction.Status.ToString(),
            //     borrowTransaction.RequestDate
            // );
            // if (!await _mailService.SendMail(CurrentUser.Email, "Borrow Response Status", body))
            // {
            //     throw new Exception("Failed to send email");
            // }
            return RedirectToAction("BorrowRequest", "Manager");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
//                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
