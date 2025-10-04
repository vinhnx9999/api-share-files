namespace VinhSharingFiles.Domain.Entities;

public class FileSharing : BaseEntity
{
    public required string FileName { get; set; }
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; }
    public bool AutoDelete { get; set; }
}
