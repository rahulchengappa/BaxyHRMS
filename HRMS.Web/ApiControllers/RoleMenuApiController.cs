using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.ApiControllers
{
    [ApiController]
    [Route("api/role-menus")]
    [Authorize(Roles = "SuperAdmin")]
    public class RoleMenuApiController : ControllerBase
    {
        private readonly IRoleMenuService _service;
        private readonly IIdProtector _idProtector;

        public RoleMenuApiController(
            IRoleMenuService service,
            IIdProtector idProtector)
        {
            _service = service;
            _idProtector = idProtector;
        }

        

        //  MENUS
        [HttpGet("GetMenu")]
        public IActionResult GetMenus()
        {
            var menus = _service.GetMenus();

            var response = menus.Select(m => new
            {
                MenuId = _idProtector.Protect(m.MenuId.ToString()),
                m.MenuName
            });

            return Ok(response);
        }

        //  MENUS BY ROLE 
        [HttpGet("GetMenuByRoleId")]
        public IActionResult GetMenusByRole(string roleId, string roleName)
        {
            int decryptedRoleId;
            try
            {
                decryptedRoleId = int.Parse(_idProtector.Unprotect(roleId));
            }
            catch
            {
                return BadRequest("Invalid roleId");
            }

            return Ok(_service.GetMenusByRole(decryptedRoleId, roleName));
        }

        //  ASSIGNED MENU IDS 
        [HttpGet("assigned/{roleId}")]
        public IActionResult GetAssignedMenuIds(string roleId)
        {
            int decryptedRoleId;
            try
            {
                decryptedRoleId = int.Parse(_idProtector.Unprotect(roleId));
            }
            catch
            {
                return BadRequest("Invalid roleId");
            }

            var menuIds = _service.GetAssignedMenuIds(decryptedRoleId);

            var encryptedMenuIds = menuIds
                .Select(id => _idProtector.Protect(id.ToString()))
                .ToList();

            return Ok(encryptedMenuIds);
        }

        // SAVE ROLE-MENU MAPPING 
        [HttpPost("SaveRoleMenuMapping")]
        public IActionResult Save([FromBody] RoleMenuMappingRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            int roleId;
            try
            {
                roleId = int.Parse(_idProtector.Unprotect(request.RoleId));
            }
            catch
            {
                return BadRequest("Invalid RoleId");
            }

            request.MenuIds ??= new List<string>();

            List<int> decryptedMenuIds;
            try
            {
                decryptedMenuIds = request.MenuIds
                    .Select(id => int.Parse(_idProtector.Unprotect(id)))
                    .Distinct()
                    .ToList();
            }
            catch
            {
                return BadRequest("Invalid MenuIds");
            }

            //  Enforce Dashboard always assigned
            var dashboardMenuId = _service
                .GetMenus()
                .First(m => m.MenuName == "Dashboard")
                .MenuId;

            if (!decryptedMenuIds.Contains(dashboardMenuId))
                decryptedMenuIds.Add(dashboardMenuId);

            _service.SaveRoleMenuMapping(roleId, decryptedMenuIds);

            return Ok(new
            {
                message = "Role menu mapping updated successfully"
            });
        }
    }
}
