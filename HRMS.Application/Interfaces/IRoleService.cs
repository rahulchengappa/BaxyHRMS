using HRMS.Domain.Entities;
namespace HRMS.Application.Interfaces
{
    public interface IRoleService
    {
        IEnumerable<RoleDto> GetAll();
        void Create(string roleName);
    }

}
