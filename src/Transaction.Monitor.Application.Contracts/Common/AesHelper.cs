using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Transaction.Monitor.Common;

public static class AesHelper
{
    const int KeySize = 256;
    public static bool Check<T>(ParamDto<T> param, string key)
    {
        string json = JsonSerializer.Serialize(param.Data);
        string shouldBe = Encrypt(json, key);
        return shouldBe == param.Signature;
    }
    
    public static string GenerateKey()
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }
    
    public static string Encrypt(string plainText, string key)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.ECB; 
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Convert.FromBase64String(key);

            ICryptoTransform encryptor = aes.CreateEncryptor();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }
    }
}