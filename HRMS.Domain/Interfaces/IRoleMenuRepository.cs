using HRMS.Domain.Entities;
using System.Collections.Generic;


namespace HRMS.Domain.Interfaces
{
    public interface IRoleMenuRepository
    {
        IEnumerable<RoleDto> GetRoles();
        IEnumerable<MenuDto> GetMenus();

        IEnumerable<MenuDto> GetMenusByRole(int roleId);

        IEnumerable<int> GetAssignedMenuIds(int roleId);

        void SaveRoleMenuMapping(int roleId, IEnumerable<int> menuIds);

    }
}
