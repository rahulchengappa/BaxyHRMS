using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public List<DepartmentDto> GetAll()
    {
        return _repository.GetAll();
    }

    public void Add(DepartmentDto dto)
    {
        _repository.Add(dto);
    }
}
