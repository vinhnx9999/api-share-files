namespace VinhSharingFiles.APIs.Models
{
    public class TextFileModel
    {
        public required string TextData { get; set; }
        public bool? DeleteAfterAccessed { get; set; } = false;
    }

    public class InputFileModel
    {
        public required IFormFile FileData { get; set; }
        public bool? DeleteAfterDownload { get; set; } = false;
    }
}
