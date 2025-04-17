using Microsoft.AspNetCore.Mvc;
using Project.AppContext;
using Project.Core;
using Project.Core.Extensions;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

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
    public async Task<ActionResult> SignUp(IFormCollection form)
    {
        string name = form["name"];
        string email = form["email"];
        string password = form["password"];
        string confirmPassword = form["confirmPassword"];
        string role = form["role"];
        User user = new User()
        {
            Id = Guid.NewGuid(),
            Created = DateTime.Now,
            Updated = DateTime.Now,
            Name = name,
            Email = email,
            Password = password,
            Role = role == "manager" ? UserType.Manager : UserType.Lecturer,
            LoginType = LoginType.Standard,
        };
        bool isCreated = await _authService.SignUp(user);

        return RedirectToAction(nameof(SignUp));
    }
}
