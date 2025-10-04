using System.Security.Cryptography;
using System.Text;

namespace VinhSharingFiles.Application.Services;

public class EncryptionService
{
    private readonly byte[] _keyEncrypt;
    private readonly byte[] _salt;

    // Constructor to initialize the password and salt        
    public EncryptionService(string keyString, string salt)
    {
        // Convert the key and IV strings to byte arrays
        _keyEncrypt = Encoding.UTF8.GetBytes(keyString);
        _salt = Encoding.UTF8.GetBytes(salt);

        // Ensure key and IV are of appropriate length for AES
        // For AES-256, key length should be 32 bytes and IV length 16 bytes.
        if (_keyEncrypt.Length != 32 || _salt.Length != 16)
        {
            throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes for AES-256.");
        }
    }

    // Encrypts a plaintext string using AES with a password and salt.
    public string Encrypt(string plainText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = _keyEncrypt;
        aesAlg.IV = _salt;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = _keyEncrypt;
        aesAlg.IV = _salt;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}
