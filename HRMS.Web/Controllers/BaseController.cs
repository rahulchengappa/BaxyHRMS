using HRMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace HRMS.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IRoleMenuService RoleMenuService =>
            HttpContext.RequestServices.GetRequiredService<IRoleMenuService>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            base.OnActionExecuting(context);
        }

        //  AUTH (JWT BASED)

        protected bool IsUserLoggedIn()
        {
            return User?.Identity?.IsAuthenticated == true;
        }

        protected int GetEmployeeId()
        {
            return int.TryParse(
                User.FindFirst("EmployeeId")?.Value,
                out int id
            ) ? id : 0;
        }


        protected string GetEmployeeName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ??
                   User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value ??
                   "";
        }

        protected int GetRoleId()
        {
            return int.TryParse(
                User.FindFirst("RoleId")?.Value,
                out int roleId
            ) ? roleId : 0;
        }

        protected string GetRoleName()
        {
            return User.FindFirst("RoleName")?.Value ?? "";
        }


        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Index", "Login");
        }

        // MENU AUTH 

        protected IActionResult? RequireMenuAccess(string menuUrl)
        {
            var roleId = GetRoleId();
            if (roleId == 0)
                return RedirectToAction("AccessDenied", "Home");

            var roleName = GetRoleName();

            // SUPERADMIN BYPASS
            if (roleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                return null;

            var menus = RoleMenuService.GetMenusForAuthorization(roleId, roleName);

            bool allowed = menus.Any(m =>
                !string.IsNullOrWhiteSpace(m.MenuUrl) &&
                (
                    menuUrl.Equals(m.MenuUrl, StringComparison.OrdinalIgnoreCase) ||
                    menuUrl.StartsWith(m.MenuUrl + "/", StringComparison.OrdinalIgnoreCase)
                )
            );


            if (!allowed)
                return RedirectToAction("AccessDenied", "Home");

            return null;
        }


        // LOGOUT 

        protected void LogoutUser()
        {
            HttpContext.Response.Cookies.Delete("access_token");
        }
    }
}
