using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        IUserService userService,
        IBorrowTransactionService borrowTransactionService
    ) : BaseController(logger: logger)
    {
        private readonly MailService _mailService = mailService;
        private readonly IItemService _itemService = itemService;
        private readonly IUserService _userService = userService;
        private readonly IBorrowTransactionService _borrowTransactionService =
            borrowTransactionService;

        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> BorrowRequest()
        {
            var items = await _borrowTransactionService.GetItems();
            var filterList = items.Where(item => item.Status == ItemStatus.Pending).ToList();
            return View(filterList);
        }

        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> BorrowingItemList(string? status)
        {
            var items = await _borrowTransactionService.GetItems();
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<ItemStatus>(status, out var parsedStatus))
                {
                    items = items.Where(i => i.Status == parsedStatus).ToList();
                }
            }
            ViewBag.SelectedStatus = status ?? "All";
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> BorrowResponse(Guid itemId, ItemStatus status)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            await using var transaction = await _borrowTransactionService.BeginTransactionAsync();
            try
            {
                var borrowTransaction = await _borrowTransactionService.GetItem(itemId);
                var _userId = borrowTransaction.BorrowerId;
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

                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var id = Guid.Parse(idClaim.Value);
                borrowTransaction.ManagerId = id;
                borrowTransaction.Status = status;

                if (status == ItemStatus.Approved)
                {
                    borrowTransaction.DueDate = borrowTransaction.RequestDate.AddDays(7);
                    item.Quantity -= borrowTransaction.Quantity;
                    if (!await _itemService.EditItem(item))
                    {
                        throw new Exception("Failed to update Item");
                    }
                }

                if (!await _borrowTransactionService.EditItem(borrowTransaction))
                {
                    throw new Exception("Failed to update Borrow Transaction");
                }

                // Open For Testing Email/ Demo
                var _user = await _userService.GetUser(_userId);
                var body = _borrowTransactionService.GenerateBorrowResponseBody(
                    _user.Email,
                    borrowTransaction.Quantity,
                    borrowTransaction.Status.ToString(),
                    borrowTransaction.RequestDate
                );
                if (!await _mailService.SendMail(_user.Email, "Borrow Response Status", body))
                {
                    throw new Exception("Failed to send email");
                }

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
        public async Task<IActionResult> UpdateStatus(Guid itemId, ItemStatus status)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            await using var transaction = await _borrowTransactionService.BeginTransactionAsync();
            try
            {
                var borrowTransaction = await _borrowTransactionService.GetItem(itemId);
                var _userId = borrowTransaction.BorrowerId;
                if (borrowTransaction == null)
                {
                    _logger.LogInformation("Borrow Transaction not found");
                    return RedirectToAction("Index", "Manager");
                }

                var item = await _itemService.GetItem(borrowTransaction.Item.Id);
                if (status == ItemStatus.Returned || status == ItemStatus.Cancelled)
                {
                    item.Quantity += borrowTransaction.Quantity;
                    if (!await _itemService.EditItem(item))
                    {
                        throw new Exception("Failed to update Item");
                    }
                }

                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var id = Guid.Parse(idClaim.Value);
                borrowTransaction.ManagerId = id;
                borrowTransaction.Status = status;

                if (!await _borrowTransactionService.EditItem(borrowTransaction))
                {
                    throw new Exception("Failed to update Borrow Transaction");
                }

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    //RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
