using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
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
    public ActionResult SignIn(IFormCollection form)
    {
        string email = form["email"];
        string password = form["password"];
        _logger.LogInformation("Email: {Email}, Password: {Password}", email, password);
        // Thêm logic xử lý đăng nhập ở đây
        return RedirectToAction(nameof(SignIn));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SignUp(IFormCollection form)
    {
        return RedirectToAction(nameof(SignUp));
    }
}
