using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Employee
{
    public class EmployeeCreateRequestDto
    {
        [Required]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter valid 10-digit mobile number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int DesignationId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public DateTime JoiningDate { get; set; }

        public bool Status { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        
        [Required]
        public string Password { get; set; } = string.Empty;

        public int? ReportingManagerId { get; set; }
    }
}