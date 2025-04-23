using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;

namespace Project.Controllers
{
    public class ProfileController(ILogger<ProfileController> logger, IItemService itemService)
        : BaseController(logger: logger)
    {
        private readonly IItemService _itemService = itemService;

        public async Task<IActionResult> Index()
        {
            var items = await _itemService.GetItems();
            ViewData["Items"] = items;
            return View(CurrentUser);
        }
    }
}
