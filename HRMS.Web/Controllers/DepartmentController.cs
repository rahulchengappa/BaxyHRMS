using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HRMS.Web.Controllers
{
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // LIST
        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Department/Index");
            if (auth != null) return auth;

            var departments = _departmentService.GetAll();
            return View(departments);
        }

        // CREATE (GET) 
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Department/Create");
            if (auth != null) return auth;

            return View(new DepartmentDto());
        }

        // CREATE (POST) 
        [HttpPost]
        public IActionResult Create(DepartmentDto dto)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Department/Create");
            if (auth != null) return auth;

            if (!ModelState.IsValid)
                return View(dto);

            dto.DepartmentId = 0;

            try
            {
                _departmentService.Add(dto);
                TempData["DepartmentSuccess"] = "Department created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                var errorMessage = ex.Message;

                if (errorMessage.Contains("Department already exists"))
                {
                    ModelState.AddModelError("", "Department already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong.");
                }

                return View(dto);
            }
        }
    }
}