using HRMS.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HRMS.Web.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IAccountRepository _accountRepository;


    public ProfileController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // PROFILE PAGE 
        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            int employeeId = GetEmployeeId(); //  JWT

            var user = _accountRepository.GetEmployeeById(employeeId);
            if (user == null)
                return RedirectToLogin();

            string photoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/profile.jpg"
            );

            ViewBag.PhotoToShow = System.IO.File.Exists(photoPath)
                ? "/uploads/profile.jpg"
                : "https://cdn-icons-png.flaticon.com/512/149/149071.png";

            return View(user);
        }

        // CHANGE PASSWORD (GET) 
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            return View();
        }

        // CHANGE PASSWORD (POST) 
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            if (string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.Message = "Password fields are required!";
                return View();
            }

            if (oldPassword == newPassword)
            {
                ViewBag.Message = "New password cannot be the same as the old password.";
                return View();
            }

            // PASSWORD COMPLEXITY VALIDATION
            if (!Regex.IsMatch(newPassword,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$"))
            {
                ViewBag.Message = "Password must be at least 8 characters and include uppercase, lowercase, number and special character.";
                return View();
            }

            int employeeId = GetEmployeeId();

            var user = _accountRepository.GetEmployeeById(employeeId);
            if (user == null)
            {
                ViewBag.Message = "User not found!";
                return View();
            }

            var loginCheck = _accountRepository.Login(user.Username!, oldPassword);

            if (loginCheck == null)
            {
                ViewBag.Message = "Old password is incorrect!";
                return View();
            }

            bool updated = _accountRepository.ResetPassword(user.Email!, newPassword);

            if (updated)
            {
                TempData["ProfileSuccess"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Message = "Something went wrong!";
            return View();
        }

        // UPLOAD PHOTO 
        [HttpPost]
        public IActionResult UploadPhoto(IFormFile photo)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            if (photo == null || photo.Length == 0)
            {
                TempData["ProfileError"] = "Invalid file upload!";
                return RedirectToAction(nameof(Index));
            }

            string folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, "profile.jpg");

            using var fs = new FileStream(filePath, FileMode.Create);
            photo.CopyTo(fs);

            TempData["ProfileSuccess"] = "Photo uploaded!";
            return RedirectToAction(nameof(Index));
        }

        // REMOVE PHOTO
        [HttpPost]
        public IActionResult RemovePhoto()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/profile.jpg"
            );

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            TempData["ProfileSuccess"] = "Photo removed!";
            return RedirectToAction(nameof(Index));
        }
    }


}
