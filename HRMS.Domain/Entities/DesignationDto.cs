namespace HRMS.Domain.Entities
{
    public class DesignationDto
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
