using HRMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HRMS.Web.Controllers
{
    [AllowAnonymous]
    public class ForgotPasswordController : BaseController
    {
        private readonly IAccountService _accountService;


    public ForgotPasswordController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // STEP 1 – Show page
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // STEP 2 – Handle form POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Email is required";
                return View();
            }

            string resetUrl =
                $"{Request.Scheme}://{Request.Host}/ForgotPassword/Reset?token=";

            bool ok = _accountService.SendResetLink(email, resetUrl);

            if (!ok)
            {
                ViewBag.Error = "Email not found";
                return View();
            }

            TempData["Success"] = "Reset link sent to your email";
            return RedirectToAction(nameof(Index));
        }

        // STEP 3 – Reset Page
        [HttpGet]
        public IActionResult Reset(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid reset token");

            return View(model: token);
        }

        // STEP 4 – Save Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPasswordPost(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.Error = "Invalid request";
                return View("Reset", token);
            }

            // PASSWORD COMPLEXITY VALIDATION
            if (!Regex.IsMatch(newPassword,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$"))
            {
                ViewBag.Error = "Password must be at least 8 characters and include uppercase, lowercase, number and special character.";
                return View("Reset", token);
            }

            bool ok = _accountService.ResetPassword(token, newPassword);

            if (!ok)
            {
                ViewBag.Error = "Invalid or expired link";
                return View("Reset", token);
            }

            return RedirectToAction("Index", "Login");
        }
    }


}
