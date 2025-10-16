namespace VinhSharingFiles.Application.DTOs;

public record FileObjectDto
{
    public int? FileId { get; set; }
    public string? Name { get; set; }
    public string? PresignedUrl { get; set; }
    public Stream Data { get; set; } = Stream.Null;
    public string? ContentType { get; set; }
    //For STORE_TEXT_IN_DB
    public string? Description { get; set; }
}
