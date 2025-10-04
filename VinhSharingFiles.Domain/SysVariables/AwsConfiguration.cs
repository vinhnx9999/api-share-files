namespace VinhSharingFiles.Domain.SysVariables;

public record AwsConfiguration
{
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? Region { get; set; }
    public string? BucketName { get; set; }
}
