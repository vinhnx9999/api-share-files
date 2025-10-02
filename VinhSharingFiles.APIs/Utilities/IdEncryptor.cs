using System.Security.Cryptography;
using System.Text;

namespace VinhSharingFiles.APIs.Utilities
{
    public static class IdEncryptor
    {
        // A secret key for encryption (should be securely stored and managed)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("secretQXltkEm2Qe"); // 32 bytes for AES-256

        public static string EncryptId(int id)
        {
            string plainText = id.ToString();
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.GenerateIV(); // Generate a new IV for each encryption
            byte[] iv = aesAlg.IV;
            using MemoryStream msEncrypt = new();
            msEncrypt.Write(iv, 0, iv.Length); // Prepend IV to the encrypted data

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using StreamWriter swEncrypt = new(csEncrypt);
                swEncrypt.Write(plainText);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static int DecryptId(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;

            // Extract IV (first 16 bytes for AES)
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new();
            // Create a MemoryStream from the encrypted data (excluding IV)
            using MemoryStream encryptedDataStream = new(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using CryptoStream csDecrypt = new(encryptedDataStream, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            string decryptedText = srDecrypt.ReadToEnd();
            return int.Parse(decryptedText);
        }
    }
}
