using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;

namespace Project.Controllers
{
    public class ProfileController(
        ILogger<ProfileController> logger,
        IUserService userService,
        IItemService itemService
    ) : BaseController(logger: logger)
    {
        private readonly IUserService _userService = userService;
        private readonly IItemService _itemService = itemService;

        public async Task<IActionResult> Index()
        {
            var items = await _itemService.GetItems();
            ViewData["Items"] = items;
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var id = Guid.Parse(idClaim.Value);
            var user = await _userService.GetUser(id);
            ViewData["CurrentUser"] = user;
            return View();
        }
    }
}
