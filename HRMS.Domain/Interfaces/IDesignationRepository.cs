using HRMS.Domain.Entities;
using System.Collections.Generic;

namespace HRMS.Domain.Interfaces
{
    public interface IDesignationRepository
    {
        List<DesignationDto> GetAll();
        void Add(DesignationDto dto);
    }
}
