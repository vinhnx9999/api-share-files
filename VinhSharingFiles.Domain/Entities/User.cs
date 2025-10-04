namespace VinhSharingFiles.Domain.Entities;

public class User: BaseEntity
{
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedDate { get; set; }
    public bool IsActive { get; set; } = false;
    public string ActiveCode { get; set; } = string.Empty;
}
