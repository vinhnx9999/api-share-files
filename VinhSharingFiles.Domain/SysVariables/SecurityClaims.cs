namespace VinhSharingFiles.Domain.SysVariables;

public static class SecurityClaims
{
    public static string Scopes { get; set; } = @"openId, profile";
    public static string System { get; set; } = @"Sharing Files System";

    public static int MaximumExpirationTime = 60;
}
