namespace HRMS.Application.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public string EmployeeID { get; set; } = string.Empty;   // FIXED
        public string EmployeeCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public string DepartmentId { get; set; } = string.Empty;   // FIXED
        public string? DepartmentName { get; set; }

        public string DesignationId { get; set; } = string.Empty;  // FIXED
        public string? DesignationName { get; set; }

        public string RoleId { get; set; } = string.Empty;         // FIXED
        public string? RoleName { get; set; }

        public DateTime JoiningDate { get; set; }
        public bool Status { get; set; }
    }
}