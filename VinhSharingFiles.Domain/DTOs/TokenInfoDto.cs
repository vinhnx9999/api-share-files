namespace VinhSharingFiles.Domain.DTOs
{
    public class TokenInfoDto
    {
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string AccessToken { get; set; } = "";
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public int ExpirationTime { get; set; } = 60;
    }
}
