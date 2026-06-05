using HRMS.Application.DTO_s.Employee;
using HRMS.Application.DTOs.Employee;
using HRMS.Domain.Entities;

namespace HRMS.Application.Interfaces
{
    public interface IEmployeeService
    {
        // AUTH
        EmployeeDto? Login(string username, string password);

        // READ
        List<EmployeeDto> GetAll();
        EmployeeDto? GetById(int id);

        // CREATE
        void Create(EmployeeCreateRequestDto request, string createdBy);

        // UPDATE (SINGLE SOURCE OF TRUTH)
        void UpdateEmployee(EmployeeDto employee);

        // DELETE
        void Delete(int id, string deletedBy);

        //Filter
        List<EmployeeDto> Filter(string filterBy, string filterValue);

        void UpdateStatus(int employeeId, bool status);

        List<EmployeeDocumentDto> GetEmployeeDocuments(int employeeId);

        void UploadEmployeeDocument(int employeeId, string documentType, string fileName, string filePath);

        void DeleteEmployeeDocument(int documentId, int employeeId);

        List<object[]> GetOrgChart(int employeeId);

        DashboardDto GetDashboardData();

    }
}
