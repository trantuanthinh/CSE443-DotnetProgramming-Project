using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Core;
using Project.Models;

namespace Project.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
            : base() { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
