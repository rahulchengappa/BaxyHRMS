namespace HRMS.Domain.Entities
{
    public class RoleMenuMappingRequest
    {
        public string RoleId { get; set; } = string.Empty;
        public List<string> MenuIds { get; set; } = new();
    }
}
