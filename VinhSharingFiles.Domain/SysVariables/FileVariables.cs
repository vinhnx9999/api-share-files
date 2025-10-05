namespace VinhSharingFiles.Domain.SysVariables;

public record FileVariables
{
    public const int MAX_LENGTH_STORE_TEXT_IN_DB = 2500;
    public const string STORE_TEXT_IN_DB = "STORE_TEXT_IN_DB";
    public const int MAX_FILE_SIZE_IN_MB = 150; // In MB
    public const int PART_SIZE_UPLOADING_IN_MB = 5; // In MB
}