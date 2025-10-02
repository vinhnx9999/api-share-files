namespace VinhSharingFiles.Domain.Entities
{

    public class User: BaseEntity
    {
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
