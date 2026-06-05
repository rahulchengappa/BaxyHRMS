using HRMS.Domain.Entities;

namespace HRMS.Web.Models
{
    public class RoleMenuViewModel
    {
        public List<RoleDto> Roles { get; set; } = new();
        public List<MenuDto> Menus { get; set; } = new();
        public List<int> AssignedMenuIds { get; set; } = new();

        public int SelectedRoleId { get; set; }
    }
}
