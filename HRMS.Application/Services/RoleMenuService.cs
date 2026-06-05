using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;

namespace HRMS.Application.Services
{
    public class RoleMenuService : IRoleMenuService
    {
        private readonly IRoleMenuRepository _repo;

        public RoleMenuService(IRoleMenuRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<RoleDto> GetRoles()
            => _repo.GetRoles();

        public IEnumerable<MenuDto> GetMenus()
            => _repo.GetMenus();

        //  USED FOR AUTHORIZATION 
        public IEnumerable<MenuDto> GetMenusForAuthorization(int roleId, string roleName)
        {
            if (roleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                return _repo.GetMenus();

            return _repo.GetMenusByRole(roleId);
        }

        //  USED ONLY FOR SIDEBAR 
        public IEnumerable<MenuDto> GetMenusByRole(int roleId, string roleName)
        {
            var menus = GetMenusForAuthorization(roleId, roleName);

            return menus.Where(m =>
                !string.IsNullOrWhiteSpace(m.MenuUrl) &&
                !m.MenuUrl.Contains("/Employee/Details", StringComparison.OrdinalIgnoreCase) &&
                !m.MenuUrl.Contains("/Employee/Edit", StringComparison.OrdinalIgnoreCase) &&
                !m.MenuUrl.Contains("/Employee/Delete", StringComparison.OrdinalIgnoreCase) &&
                !m.MenuUrl.Contains("/Employee/UpdateStatus", StringComparison.OrdinalIgnoreCase)
            );
        }

        public IEnumerable<int> GetAssignedMenuIds(int roleId)
            => _repo.GetAssignedMenuIds(roleId);

        public void SaveRoleMenuMapping(int roleId, IEnumerable<int> menuIds)
            => _repo.SaveRoleMenuMapping(roleId, menuIds);
    }
}
