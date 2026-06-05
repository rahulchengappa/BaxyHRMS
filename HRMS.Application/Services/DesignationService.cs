using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;

public class DesignationService : IDesignationService
{
    private readonly IDesignationRepository _repository;

    public DesignationService(IDesignationRepository repository)
    {
        _repository = repository;
    }

    public List<DesignationDto> GetAll()
    {
        return _repository.GetAll();
    }

    public void Add(DesignationDto dto)
    {
        _repository.Add(dto);
    }
}
