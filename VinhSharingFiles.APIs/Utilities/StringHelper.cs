namespace VinhSharingFiles.APIs.Utilities
{
    public static class StringHelper
    {
        public static int ConvertingHex2Int(this string? value, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            try
            {
                return Convert.ToInt32(value, 16);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
