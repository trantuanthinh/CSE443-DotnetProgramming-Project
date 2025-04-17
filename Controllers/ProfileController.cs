using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

namespace Project.Controllers
{
    public class ProfileController(ILogger<ProfileController> logger)
        : BaseController(logger: logger)
    {
        public async Task<IActionResult> Index()
        {
            return View(CurrentUser);
        }
    }
}
