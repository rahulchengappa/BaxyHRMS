namespace HRMS.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; } 
    }
}

