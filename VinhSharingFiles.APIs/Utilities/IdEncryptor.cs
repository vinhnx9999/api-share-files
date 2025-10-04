using System;
using System.Text;

namespace VinhSharingFiles.APIs.Utilities;

public static class IdEncryptor
{
    public static string EncryptId(int id)
    {
        Random random = new();
        string plainText = id.ToString("X2");            
        return $"{GenerateRandomString(6)}{plainText}{random.NextInt64(99):00}";
    }

    public static int DecryptId(string cipherText)
    {
        try
        {
            string hex = cipherText[6..];
            hex = hex[..^2]; // Remove the last two characters
            return hex.ConvertingHex2Int(0);
        }
        catch
        {
            return 0;
        }
    }

    private static string GenerateRandomString(int length)
    {
        // Define character sets
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";

        // Build the allowed character set based on parameters
        StringBuilder allowedChars = new();
        allowedChars.Append(lowerCaseChars);
        allowedChars.Append(upperCaseChars);
        allowedChars.Append(numberChars);

        if (allowedChars.Length == 0)
        {
            throw new ArgumentException("At least one character type must be included.");
        }

        // Generate the random string
        Random random = new();
        StringBuilder result = new(length);
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(allowedChars.Length);
            result.Append(allowedChars[index]);
        }

        return result.ToString();
    }
}
