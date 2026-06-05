using HRMS.Domain.Entities;
using System.Collections.Generic;

namespace HRMS.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        List<DepartmentDto> GetAll();
        void Add(DepartmentDto dto);
    }
}
