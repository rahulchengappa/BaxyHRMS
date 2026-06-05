using HRMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HRMS.Application.Interfaces; // ✅ ADDED

namespace HRMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService; // ✅ ADDED

        public HomeController(ILogger<HomeController> logger,
                              IEmployeeService employeeService) // ✅ ADDED
        {
            _logger = logger;
            _employeeService = employeeService; // ✅ ADDED
        }

        // ACCESS DENIED 
        public IActionResult AccessDenied()
        {
            return View();
        }

        // DASHBOARD 
        public IActionResult Dashboard()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            // RBAC via menu
            var auth = RequireMenuAccess("/Home/Dashboard");
            if (auth != null) return auth;

            //  ADDED: fetch dashboard data
            var dashboardData = _employeeService.GetDashboardData();

            return View(dashboardData); //  CHANGED
        }

        // HOME / INDEX
        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            ViewBag.EmployeeName = GetEmployeeName();
            ViewBag.RoleName = GetRoleName();

            return View();
        }

        // PRIVACY 
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // ERROR
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}