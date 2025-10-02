namespace VinhSharingFiles.APIs.Models
{
    public class TextFileModel
    {
        public required string TextData { get; set; }
        public bool? DeleteAfterAccessed { get; set; } = false;
    }
}
