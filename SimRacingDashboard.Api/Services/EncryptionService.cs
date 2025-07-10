using System;
using System.Security.Cryptography;
using System.Text;

namespace SimRacingDashboard.Api.Services;

public class EncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(string keyBase64, string ivBase64)
    {
        _key = Convert.FromBase64String(keyBase64);
        _iv = Convert.FromBase64String(ivBase64);

        // Validate key and IV lengths
        if (_key.Length != 32) // AES-256 requires a 32-byte key
            throw new ArgumentException("Key must be 32 bytes (256 bits) long.");
        if (_iv.Length != 16) // AES requires a 16-byte IV
            throw new ArgumentException("IV must be 16 bytes (128 bits) long.");
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor();
        var cipherBytes = Convert.FromBase64String(encryptedText);

        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }

    public static (string Key, string IV) GenerateKeyAndIV()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();
        return (Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
    }
}
