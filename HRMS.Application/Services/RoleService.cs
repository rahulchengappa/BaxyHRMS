using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;

namespace HRMS.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;

        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<RoleDto> GetAll()
            => _repo.GetAll();

        public void Create(string roleName)
        {
            //  BUSINESS RULE GUARD
            if (_repo.RoleExists(roleName))
            {
                throw new InvalidOperationException("Role already exists.");
            }

            _repo.Insert(roleName);
        }
    }
}
