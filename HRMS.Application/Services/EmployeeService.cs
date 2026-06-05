using HRMS.Application.DTOs.Employee;
using HRMS.Application.Interfaces;
using HRMS.Domain.Entities;
using HRMS.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace HRMS.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        // AUTH
        public EmployeeDto? Login(string username, string password)
        {
            return _repo.Login(username, password);
        }

        // READ
        public List<EmployeeDto> GetAll()
        {
            return _repo.GetAllEmployees();
        }

        public EmployeeDto? GetById(int id)
        {
            return _repo.GetEmployee(id);
        }

        // CREATE
        public void Create(EmployeeCreateRequestDto request, string createdBy)
        {
            var emp = new EmployeeDto
            {
                EmployeeCode = request.EmployeeCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                DepartmentId = request.DepartmentId,
                DesignationId = request.DesignationId,
                RoleId = request.RoleId,
                JoiningDate = request.JoiningDate,
                Status = request.Status,
                Username = request.Username,
                Password = request.Password,
                ReportingManagerId = request.ReportingManagerId,
                CreatedBy = createdBy
            };

            try
            {
                _repo.CreateEmployee(emp);
            }
            catch (Exception ex)   // ✅ FIXED
            {
                if (ex.Message.Contains("already exists"))
                {
                    throw new InvalidOperationException(ex.Message);
                }

                throw;
            }
        }

        public DashboardDto GetDashboardData()
        {
            return _repo.GetDashboardData();
        }

        // UPDATE
        public void UpdateEmployee(EmployeeDto updated)
        {
            var existing = _repo.GetEmployee(updated.EmployeeID);
            if (existing == null)
            {
                throw new InvalidOperationException("Employee not found.");
            }

            string updatedBy = string.IsNullOrWhiteSpace(updated.UpdatedBy)
                ? "SYSTEM"
                : updated.UpdatedBy;

            void Log(string field, string? oldVal, string? newVal)
            {
                if (oldVal == newVal) return;

                _repo.InsertAudit(new EmployeeAuditDto
                {
                    EmployeeID = updated.EmployeeID,
                    FieldName = field,
                    OldValue = oldVal,
                    NewValue = newVal,
                    UpdatedBy = updatedBy
                });
            }

            Log("FirstName", existing.FirstName, updated.FirstName);
            Log("LastName", existing.LastName, updated.LastName);
            Log("Email", existing.Email, updated.Email);
            Log("MobileNumber", existing.MobileNumber, updated.MobileNumber);
            Log("DepartmentId", existing.DepartmentId.ToString(), updated.DepartmentId.ToString());
            Log("DesignationId", existing.DesignationId.ToString(), updated.DesignationId.ToString());
            Log("RoleId", existing.RoleId.ToString(), updated.RoleId.ToString());
            Log("JoiningDate",
                existing.JoiningDate.ToString("yyyy-MM-dd"),
                updated.JoiningDate.ToString("yyyy-MM-dd"));
            Log("Status",
                existing.Status ? "Active" : "Inactive",
                updated.Status ? "Active" : "Inactive");

            existing.EmployeeCode = updated.EmployeeCode;
            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.Email = updated.Email;
            existing.MobileNumber = updated.MobileNumber;
            existing.DepartmentId = updated.DepartmentId;
            existing.DesignationId = updated.DesignationId;
            existing.RoleId = updated.RoleId;
            existing.JoiningDate = updated.JoiningDate;
            existing.Status = updated.Status;
            existing.ReportingManagerId = updated.ReportingManagerId;
            existing.UpdatedBy = updatedBy;

            try
            {
                _repo.UpdateEmployee(existing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        // DELETE
        public void Delete(int id, string deletedBy)
        {
            _repo.DeleteEmployee(id, deletedBy);
        }

        public List<EmployeeDto> Filter(string filterBy, string filterValue)
        {
            return _repo.Filter(filterBy, filterValue);
        }

        public void UpdateStatus(int employeeId, bool status)
        {
            _repo.UpdateStatus(employeeId, status);
        }

        public List<EmployeeDocumentDto> GetEmployeeDocuments(int employeeId)
        {
            return _repo.GetEmployeeDocuments(employeeId);
        }

        public void UploadEmployeeDocument(int employeeId, string documentType, string fileName, string filePath)
        {
            _repo.UploadEmployeeDocument(employeeId, documentType, fileName, filePath);
        }

        public void DeleteEmployeeDocument(int documentId, int employeeId)
        {
            _repo.DeleteEmployeeDocument(documentId, employeeId);
        }

        // ORG CHART
        public List<object[]> GetOrgChart(int employeeId)
        {
            var all = _repo.GetAllEmployees();
            var result = new List<object[]>();

            result.Add(new object[] { "Name", "Manager" });

            var current = all.FirstOrDefault(x => x.EmployeeID == employeeId);
            if (current == null) return result;

            var hierarchy = new List<EmployeeDto>();

            while (current != null)
            {
                hierarchy.Insert(0, current);
                current = all.FirstOrDefault(x => x.EmployeeID == current.ReportingManagerId);
            }

            for (int i = 0; i < hierarchy.Count; i++)
            {
                var emp = hierarchy[i];

                string? managerName = i == 0
                    ? null
                    : FormatName(hierarchy[i - 1], all);

                result.Add(new object[]
                {
                    FormatName(emp, all),
                    managerName ?? ""
                });
            }

            var clicked = hierarchy.Last();

            var children = all
                .Where(x => x.ReportingManagerId == clicked.EmployeeID)
                .ToList();

            foreach (var child in children)
            {
                AddSubordinates(child, all, result, FormatName(clicked, all));
            }

            return result;
        }

        private void AddSubordinates(
            EmployeeDto emp,
            List<EmployeeDto> allEmployees,
            List<object[]> result,
            string? managerName)
        {
            result.Add(new object[]
            {
                FormatName(emp, allEmployees),
                managerName ?? ""
            });

            var children = allEmployees
                .Where(x => x.ReportingManagerId == emp.EmployeeID)
                .ToList();

            foreach (var child in children)
            {
                AddSubordinates(child, allEmployees, result, FormatName(emp, allEmployees));
            }
        }

        private string FormatName(EmployeeDto emp, List<EmployeeDto> all)
        {
            bool hasChildren = all.Any(x => x.ReportingManagerId == emp.EmployeeID);
            bool isTop = emp.ReportingManagerId == null;

            string bgColor = "#ffc107";
            string textColor = "#000";

            if (isTop)
            {
                bgColor = "#0d6efd";
                textColor = "#fff";
            }
            else if (hasChildren)
            {
                bgColor = "#198754";
                textColor = "#fff";
            }

            return $@"
        <div style='
            background:{bgColor};
            color:{textColor};
            padding:10px;
            border-radius:8px;
            text-align:center;
            font-size:13px;
            box-shadow:0 2px 6px rgba(0,0,0,0.2);
        '>
            <b>{emp.EmployeeName}</b><br/>
            ({emp.DesignationName})
        </div>";
        }
    }
}