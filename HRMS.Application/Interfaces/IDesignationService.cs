using HRMS.Domain.Entities;
using System.Collections.Generic;

namespace HRMS.Application.Interfaces
{
    public interface IDesignationService
    {
        List<DesignationDto> GetAll();
        void Add(DesignationDto dto);
    }
}
