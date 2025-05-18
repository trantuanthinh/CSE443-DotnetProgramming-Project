using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

namespace Project.Controllers
{
    public class ProfileController(
        ILogger<ProfileController> logger,
        IUserService userService,
        IItemService itemService,
        ICategoryService categoryService,
        SharedService sharedService
    ) : BaseController(logger: logger)
    {
        private readonly IUserService _userService = userService;
        private readonly IItemService _itemService = itemService;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly SharedService _sharedService = sharedService;

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetItems();
            ViewData["Categories"] = categories;

            var items = await _itemService.GetItems();
            ViewData["Items"] = items;

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var id = Guid.Parse(idClaim.Value);
            var user = await _userService.GetUser(id);
            ViewData["CurrentUser"] = user;

            var userList = await _userService.GetLecturers();
            ViewData["Users"] = userList;

            return View();
        }

        #region Profile
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(IFormCollection form)
        {
            try
            {
                var idValue = form["id"];
                if (string.IsNullOrWhiteSpace(idValue))
                {
                    _logger.LogWarning("User ID is missing.");
                    return RedirectToAction(nameof(Index));
                }

                var id = Guid.Parse(idValue);

                var name = form["name"];
                var username = form["username"];
                var email = form["email"];
                var password = form["password"];
                var phoneNumber = form["phoneNumber"];

                var user = await _userService.GetUser(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found.");
                    return RedirectToAction(nameof(Index));
                }

                user.Name = name;
                user.Username = username;
                user.Email = email;
                user.PhoneNumber = phoneNumber;

                bool isUpdated = await _userService.EditUser(user);
                if (!isUpdated)
                {
                    _logger.LogWarning("Update failed.");
                }

                _logger.LogInformation("Profile updated successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateProfile");
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region Item
        [HttpPost]
        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> HandleItemForm(IFormCollection form)
        {
            try
            {
                var idValue = form["id"];
                Guid? id = string.IsNullOrWhiteSpace(idValue) ? null : Guid.Parse(idValue);

                var name = form["name"];
                var quantity = int.Parse(form["quantity"]);
                var categoryId = Guid.Parse(form["categoryId"]);
                var image = form.Files.GetFile("image");
                var imageBase64 =
                    image == null || image.Length == 0
                        ? string.Empty
                        : _sharedService.ImageToBase64(image);

                if (id.HasValue)
                {
                    var item = new Item
                    {
                        Id = id.Value,
                        Name = name,
                        Quantity = quantity,
                        CategoryId = categoryId,
                        Image = imageBase64,
                    };
                    bool isUpdated = await _itemService.EditItem(item);
                    if (!isUpdated)
                    {
                        return Json(new { success = false, message = "Update failed." });
                    }
                    return Json(new { success = true, message = "Item updated successfully." });
                }
                else
                {
                    id = Guid.NewGuid();
                    var item = new Item
                    {
                        Id = id.Value,
                        Name = name,
                        Quantity = quantity,
                        CategoryId = categoryId,
                        Image = imageBase64,
                    };
                    bool isCreated = await _itemService.CreateItem(item);
                    if (!isCreated)
                    {
                        return Json(new { success = false, message = "Create failed." });
                    }
                    return Json(new { success = true, message = "Item created successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HandleItemForm");
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> DeleteItem([FromBody] Guid id)
        {
            try
            {
                bool isDeleted = await _itemService.DeleteItem(id);
                if (!isDeleted)
                {
                    return Json(new { success = false, message = "Delete failed." });
                }
                return Json(new { success = true, message = "Item deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item");
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion
    }
}
