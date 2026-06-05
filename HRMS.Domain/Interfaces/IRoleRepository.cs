using HRMS.Domain.Entities;

namespace HRMS.Domain.Interfaces
{
    public interface IRoleRepository
    {
        IEnumerable<RoleDto> GetAll();
        void Insert(string roleName);
        bool RoleExists(string roleName);
    }
}
