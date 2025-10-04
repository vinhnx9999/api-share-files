namespace VinhSharingFiles.Domain.DTOs;

public record FileDto
{
    public int? Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }

    //For STORE_TEXT_IN_DB
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
