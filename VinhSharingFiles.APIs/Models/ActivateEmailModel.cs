namespace VinhSharingFiles.APIs.Models
{

    public record ActivateEmailModel
    {
        public required string Email { get; set; }
        public required string ActiveCode { get; set; }
    }
}
