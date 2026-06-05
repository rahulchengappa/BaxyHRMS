using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly IAccountService _accountService;
    private readonly JwtTokenService _jwtService;

    public LoginController(
        IAccountService accountService,
        JwtTokenService jwtService)
    {
        _accountService = accountService;
        _jwtService = jwtService;
    }

    // SHOW LOGIN PAGE
    [HttpGet]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Dashboard", "Home");

        return View();
    }

    // HANDLE LOGIN 
    [HttpPost]
    public IActionResult Index(LoginRequestDto request)
    {
        if (request == null)
            return BadRequest("Invalid login request");

        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            ViewBag.Error = "Username and password are required";
            return View("Index");
        }

        if (!ModelState.IsValid)
            return View("Index");

        var user = _accountService.Login(request);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View("Index");
        }

        // JWT CONTAINS EVERYTHING MVC NEEDS
        var token = _jwtService.GenerateToken(
            user.EmployeeId,
            user.EmployeeName ?? "",
            user.RoleId,
            user.RoleName ?? ""
        );

        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,                 // HTTPS
            SameSite = SameSiteMode.Strict
        });

        return RedirectToAction("Dashboard", "Home");
    }

    // LOGOUT 
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");

        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        return RedirectToAction("Index");
    }
}
