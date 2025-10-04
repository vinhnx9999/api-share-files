namespace VinhSharingFiles.APIs.Models
{
    public record ActivateUserNameModel
    {
        public required string UserName { get; set; }
        public required string ActiveCode { get; set; }
    }
}
