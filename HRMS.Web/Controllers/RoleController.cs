using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        // LIST 
        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Role/Index");
            if (auth != null) return auth;

            var roles = _service.GetAll();
            return View(roles);
        }

        // CREATE (GET) 
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            //  RBAC 
            var auth = RequireMenuAccess("/Role/Create");
            if (auth != null) return auth;

            return View();
        }

        // CREATE (POST) 
        [HttpPost]
        public IActionResult Create(RoleDto model)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            // RBAC via DB 
            var auth = RequireMenuAccess("/Role/Create");
            if (auth != null) return auth;

            if (model == null)
                return BadRequest("Invalid role data");

            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                ModelState.AddModelError("RoleName", "Role name is required");
                return View(model);
            }

            try
            {
                _service.Create(model.RoleName);
                TempData["RoleSuccess"] = "Role created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    ModelState.AddModelError("", "Role already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong.");
                }

                return View(model);
            }
        }
    }
}
