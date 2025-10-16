namespace VinhSharingFiles.Application.DTOs;

public record UserInfoDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsActive { get; set; } = false;
    public DateTime? ConfirmedDate { get; set; }        
    public DateTime CreatedDate { get; set; }
}
