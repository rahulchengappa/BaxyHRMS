using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using HRMS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
    public class RoleMenuController : BaseController
    {
        private readonly IRoleMenuService _roleMenuService;
        private readonly IRoleService _roleService;
        private readonly IIdProtector _idProtector;

        public RoleMenuController(
            IRoleService roleService,
            IRoleMenuService roleMenuService,
            IIdProtector idProtector)
        {
            _roleService = roleService;
            _roleMenuService = roleMenuService;
            _idProtector = idProtector;
        }

       
        // GET: Role Menu Mapping
       
        [HttpGet]
        public IActionResult Index(string roleId = "")
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

       
            var auth = RequireMenuAccess("/RoleMenu/Index");
            if (auth != null) return auth;

            int selectedRoleId = 0;

            if (!string.IsNullOrWhiteSpace(roleId))
            {
                try
                {
                    selectedRoleId = int.Parse(_idProtector.Unprotect(roleId));
                }
                catch
                {
                    return BadRequest("Invalid roleId");
                }
            }

            var roles = _roleService.GetAll().ToList();

            var model = new RoleMenuViewModel
            {
                Roles = roles,
                SelectedRoleId = selectedRoleId,

                Menus = selectedRoleId > 0
                    ? _roleMenuService.GetMenus().ToList()
                    : new List<MenuDto>(),

                AssignedMenuIds = selectedRoleId > 0
                    ? _roleMenuService.GetAssignedMenuIds(selectedRoleId).ToList()
                    : new List<int>()
            };

            return View(model);
        }

        // POST: Save Role Menu Mapping
        
        [HttpPost]
        public IActionResult Save(string roleId, List<string> menuIds)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            //  DB-driven RBAC
            var auth = RequireMenuAccess("/RoleMenu/Index");
            if (auth != null) return auth;

            int decryptedRoleId;
            try
            {
                decryptedRoleId = int.Parse(_idProtector.Unprotect(roleId));
            }
            catch
            {
                return BadRequest("Invalid roleId");
            }

            menuIds ??= new List<string>();

            List<int> decryptedMenuIds;
            try
            {
                decryptedMenuIds = menuIds
                    .Select(id => int.Parse(_idProtector.Unprotect(id)))
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();
            }
            catch
            {
                return BadRequest("Invalid menuIds");
            }

            // FORCE DASHBOARD ALWAYS PRESENT
            var dashboardMenuId = _roleMenuService
                .GetMenus()
                .First(m => m.MenuName == "Dashboard")
                .MenuId;

            if (!decryptedMenuIds.Contains(dashboardMenuId))
                decryptedMenuIds.Add(dashboardMenuId);

            _roleMenuService.SaveRoleMenuMapping(decryptedRoleId, decryptedMenuIds);

            TempData["RoleMenuSuccess"] = "Role menu mapping updated successfully";

            return RedirectToAction(nameof(Index), new
            {
                roleId = _idProtector.Protect(decryptedRoleId.ToString())
            });
        }
    }
}
