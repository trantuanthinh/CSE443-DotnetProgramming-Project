using Microsoft.AspNetCore.Mvc;
using Project.AppContext;
using Project.Core;
using Project.Core.Extensions;
using Project.Interfaces;
using Project.Models;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService,
        DataContext dataContext,
        ILogger<AuthController> logger
    )
        : base(dataContext, logger)
    {
        _authService = authService;
    }

    public ActionResult SignIn()
    {
        return View();
    }

    public ActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SignIn(IFormCollection form)
    {
        string email = form["email"];
        string password = form["password"];
        User user = await _authService.SignIn(email, password);
        if (user != null)
        {
            HttpContext.Session.SetObject("CurrentUser", user);
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SignUp(IFormCollection form)
    {
        return RedirectToAction(nameof(SignUp));
    }
}
