using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Project.AppContext;
using Project.Core;
using Project.Core.Extensions;
using Project.DTO;
using Project.Interfaces;
using Project.Models;
using Project.Utils;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService,
        DataContext dataContext,
        IMapper mapper,
        ILogger<AuthController> logger
    )
        : base(dataContext, mapper, logger)
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
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);

        // Lưu cookie đăng nhập
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

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

        return RedirectToAction(nameof(SignIn));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ContinueWithGoogle()
    {
        var url = _authService.GetGoogleUrl();
        return Json(new { redirectUrl = url });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Route("auth/google/callback")]
    public async Task<IActionResult> GoogleCallback(string code)
    {
        try
        {
            string accessToken = await _authService.ExchangeCodeForTokenAsync(code, "signin");
            var userInfo = await _authService.GetUserInfoAsync(accessToken);
            using var doc = JsonDocument.Parse(userInfo);
            string email = doc.RootElement.TryGetProperty("email", out var emailElement)
                ? emailElement.GetString()
                : "";

            User _item = await _authService.GoogleAuthenticatedUser(email);
            if (_item == null)
            {
                string name = doc.RootElement.TryGetProperty("name", out var nameElement)
                    ? nameElement.GetString()
                    : "";

                string phoneNumber = doc.RootElement.TryGetProperty(
                    "phoneNumber",
                    out var phoneNumberElement
                )
                    ? phoneNumberElement.GetString()
                    : "";

                string avatar = doc.RootElement.TryGetProperty("picture", out var avatarElement)
                    ? avatarElement.GetString()
                    : "";

                GoogleRequest item = new GoogleRequest
                {
                    Name = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                };

                _item = _mapper.Map<User>(item);
                bool isCreated = await _authService.CreateGoogleUser(_item);
                if (!isCreated)
                {
                    _logger.LogInformation("Failed to create Google User");
                    return RedirectToAction(nameof(SignIn));
                }
            }

            string script =
                $@"<script>
                    if (window.opener)
                        {{
                            window.close();
                        }}
                    </script>";
            return Content(script, "text/html");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Failed to sign in with Google: " + ex.Message);
        }
    }
}
