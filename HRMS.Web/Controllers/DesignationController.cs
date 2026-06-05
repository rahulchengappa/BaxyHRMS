using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HRMS.Web.Controllers
{
    public class DesignationController : BaseController
    {
        private readonly IDesignationService _designationService;

        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        // LIST 
        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Designation/Index");
            if (auth != null) return auth;

            var designations = _designationService.GetAll();
            return View(designations);
        }

        // CREATE (GET) 
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Designation/Create");
            if (auth != null) return auth;

            return View(new DesignationDto());
        }

        // CREATE (POST) 
        [HttpPost]
        public IActionResult Create(DesignationDto dto)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Designation/Create");
            if (auth != null) return auth;

            if (!ModelState.IsValid)
                return View(dto);

            dto.DesignationId = 0;

            try
            {
                _designationService.Add(dto);
                TempData["DesignationSuccess"] = "Designation created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                var errorMessage = ex.Message;

                if (errorMessage.Contains("Designation already exists"))
                {
                    ModelState.AddModelError("", "Designation already exists.");
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