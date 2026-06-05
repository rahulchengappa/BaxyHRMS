public class EmployeeDocumentDto
{
    public int DocumentId { get; set; }

    public int EmployeeId { get; set; }

    public string? DocumentType { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime UploadedOn { get; set; }
}