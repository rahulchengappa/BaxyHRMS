public class EmployeeAuditDto
{
    public int EmployeeID { get; set; }
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? UpdatedBy { get; set; }
}
