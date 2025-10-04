namespace VinhSharingFiles.APIs.Models
{
    public record TextFileModel
    {
        public required string TextData { get; set; }
        public bool? DeleteAfterAccessed { get; set; } = false;
    }
}
