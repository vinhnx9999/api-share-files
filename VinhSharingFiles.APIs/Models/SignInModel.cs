namespace VinhSharingFiles.APIs.Models;

public record SignInModel
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
