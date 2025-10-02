namespace VinhSharingFiles.Domain.DTOs
{
    public class FileObjectDto
    {
        public int? FileId { get; set; }
        public string? Name { get; set; }
        public string? PresignedUrl { get; set; }
        public Stream Data { get; set; } = Stream.Null;
        public string? ContentType { get; set; }
    }
}
