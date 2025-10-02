namespace VinhSharingFiles.Domain.Entities
{
    public class FileSharing : BaseEntity
    {
        public required string FileName { get; set; }
        public string? Description { get; set; }
        public required string FilePath { get; set; }
        public required string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
        public bool AutoDelete { get; set; }
    }
}
