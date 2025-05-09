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
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(IFormCollection form)
        {
            var idValue = Guid.Parse(form["id"]);
            var name = form["name"];
            var userName = form["username"];
            var phoneNumber = form["phoneNumber"];
            var password = form["password"];

            var user = await _userService.GetUser(idValue);
            if (user == null)
            {
                return BadRequest();
            }

            user.Name = name;
            user.Username = userName;
            user.PhoneNumber = phoneNumber;
            if (password != string.Empty)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            }
            bool isUpdated = await _userService.EditUser(user);
            if (!isUpdated)
            {
                return BadRequest();
            }
            return RedirectToAction("Index", "Profile");
        }
        #endregion

        #region Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> HandleItemForm(IFormCollection form)
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
                    return BadRequest();
                }
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
                    return BadRequest();
                }
            }
            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserType.Manager))]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            bool isDeleted = await _itemService.DeleteItem(id);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return RedirectToAction("Index", "Profile");
        }
        #endregion
    }
}
