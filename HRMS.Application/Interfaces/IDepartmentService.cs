using HRMS.Domain.Entities;
using System.Collections.Generic;

namespace HRMS.Application.Interfaces
{
    public interface IDepartmentService
    {
        List<DepartmentDto> GetAll();
        void Add(DepartmentDto dto);
    }
}
