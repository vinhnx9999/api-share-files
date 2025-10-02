using System.Security.Cryptography;
using System.Text;

namespace VinhSharingFiles.Application.Services
{
    //public static class EncryptionService
    //{
    //    // This constant is used to determine the keysize of the encryption algorithm in bits.
    //    // We divide this by 8 within the code below to get the equivalent number of bytes.
    //    private const int Keysize = 256;

    //    // This constant determines the number of iterations for the password bytes generation function.
    //    private const int DerivationIterations = 1000;

    //    public static string Encrypt(string plainText, string passPhrase)
    //    {
    //        // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
    //        // so that the same Salt and IV values can be used when decrypting.  
    //        var saltStringBytes = Generate256BitsOfRandomEntropy();
    //        var ivStringBytes = Generate256BitsOfRandomEntropy();
    //        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
    //        using var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations);
    //        var keyBytes = password.GetBytes(Keysize / 8);
    //        using var symmetricKey = new RijndaelManaged();
    //        //symmetricKey.BlockSize = 256;
    //        symmetricKey.Mode = CipherMode.CBC;
    //        symmetricKey.Padding = PaddingMode.PKCS7;
    //        using var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
    //        using var memoryStream = new MemoryStream();
    //        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
    //        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
    //        cryptoStream.FlushFinalBlock();
    //        // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
    //        var cipherTextBytes = saltStringBytes;
    //        cipherTextBytes = [.. cipherTextBytes, .. ivStringBytes];
    //        cipherTextBytes = [.. cipherTextBytes, .. memoryStream.ToArray()];
    //        memoryStream.Close();
    //        cryptoStream.Close();
    //        return Convert.ToBase64String(cipherTextBytes);
    //    }

    //    public static string Decrypt(string cipherText, string passPhrase)
    //    {
    //        // Get the complete stream of bytes that represent:
    //        // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
    //        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
    //        // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
    //        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
    //        // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
    //        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
    //        // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
    //        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

    //        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
    //        {
    //            var keyBytes = password.GetBytes(Keysize / 8);
    //            using (var symmetricKey = new RijndaelManaged())
    //            {
    //                symmetricKey.BlockSize = 256;
    //                symmetricKey.Mode = CipherMode.CBC;
    //                symmetricKey.Padding = PaddingMode.PKCS7;
    //                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
    //                {
    //                    using (var memoryStream = new MemoryStream(cipherTextBytes))
    //                    {
    //                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
    //                        using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
    //                        {
    //                            return streamReader.ReadToEnd();
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private static byte[] Generate256BitsOfRandomEntropy()
    //    {
    //        var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
    //        using (var rngCsp = new RNGCryptoServiceProvider())
    //        {
    //            // Fill the array with cryptographically secure random bytes.
    //            rngCsp.GetBytes(randomBytes);
    //        }
    //        return randomBytes;
    //    }
    //}

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
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _keyEncrypt;
                aesAlg.IV = _salt;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _keyEncrypt;
                aesAlg.IV = _salt;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
