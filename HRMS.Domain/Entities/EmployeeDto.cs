namespace HRMS.Domain.Entities
{
    public class EmployeeDto
    {
        public int EmployeeID { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? EmployeeName { get; set; }

        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public int DesignationId { get; set; }
        public string? DesignationName { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public DateTime JoiningDate { get; set; }
        public bool Status { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public List<EmployeeDocumentDto> Documents { get; set; } = new();

        public int? ReportingManagerId { get; set; }
        public string? ReportingManagerName { get; set; }
    }
}