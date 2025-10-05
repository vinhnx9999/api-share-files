using System.Globalization;
using VinhSharingFiles.Domain.SysVariables;

namespace VinhSharingFiles.APIs.Utilities;

public static class SensitiveDataChecker
{
    public static bool SensitiveKeywords(this string text)
    {
        if(string.IsNullOrWhiteSpace(text))
            return false;

        if (text.Length < FileVariables.MAX_LENGTH_STORE_TEXT_IN_DB)
        {
            // Check for sensitive keywords
            string lowerText = text.ToLower(new CultureInfo("en-US", false));
            return lowerText.Contains("password") ||
                   lowerText.Contains("ssn") ||
                   lowerText.Contains("email") ||
                   lowerText.Contains("confidential");
        }

        return true; // Skip checking for very large text        
    }
}
