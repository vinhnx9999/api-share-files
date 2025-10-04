using System.ComponentModel.DataAnnotations;

namespace VinhSharingFiles.APIs.Models
{
    public record SignUpModel
    {
        public required string UserName { get; set; }
        public string? DisplayName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }
    }
}
