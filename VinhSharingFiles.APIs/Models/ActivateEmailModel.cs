namespace VinhSharingFiles.APIs.Models
{
    public class ActivateUserNameModel
    {
        public required string UserName { get; set; }
        public required string ActiveCode { get; set; }
    }

    public class ActivateEmailModel
    {
        public required string Email { get; set; }
        public required string ActiveCode { get; set; }
    }
}
