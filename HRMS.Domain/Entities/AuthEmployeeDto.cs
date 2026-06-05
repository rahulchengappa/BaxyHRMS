namespace HRMS.Domain.Entities
{
    public class AuthEmployeeDto
    {
        public int EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; } = string.Empty;
    }
}