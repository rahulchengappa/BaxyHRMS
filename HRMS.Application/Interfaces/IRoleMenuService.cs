using HRMS.Domain.Entities;

namespace HRMS.Application.Interfaces
{
    public interface IRoleMenuService
    {
        IEnumerable<RoleDto> GetRoles();
        IEnumerable<MenuDto> GetMenus();
        IEnumerable<MenuDto> GetMenusForAuthorization(int roleId, string roleName);
        IEnumerable<MenuDto> GetMenusByRole(int roleId, string roleName);
        IEnumerable<int> GetAssignedMenuIds(int roleId);

        void SaveRoleMenuMapping(int roleId, IEnumerable<int> menuIds);
    }
}
