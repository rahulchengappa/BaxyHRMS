using HRMS.Domain.Entities;

namespace HRMS.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        List<EmployeeDto> GetAllEmployees();
        List<EmployeeDto> Filter(string filterBy, string filterValue);

        EmployeeDto? GetEmployee(int id);
        void CreateEmployee(EmployeeDto employee);
        void UpdateEmployee(EmployeeDto employee);
        void DeleteEmployee(int EmployeeId, string UpdatedBy);
        void InsertAudit(EmployeeAuditDto audit);
        void UpdateStatus(int employeeId, bool status);


        EmployeeDto? Login(string username, string password);

        List<EmployeeDocumentDto> GetEmployeeDocuments(int employeeId);

        void UploadEmployeeDocument(int employeeId, string documentType, string fileName, string filePath);
        void DeleteEmployeeDocument(int documentId, int employeeId);

        DashboardDto GetDashboardData();
    }
}
