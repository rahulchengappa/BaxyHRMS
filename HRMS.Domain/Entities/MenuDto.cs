namespace HRMS.Domain.Entities
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? MenuUrl { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public string MenuGroup { get; set; } = "";
        public int? ParentMenuId { get; set; }
        public int MenuOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
