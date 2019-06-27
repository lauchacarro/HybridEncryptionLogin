using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using HybridEncryptionLogin.Services.Abstracts;
using Microsoft.Extensions.Configuration;

namespace HybridEncryptionLogin.Services.Concretes
{
    public class AESService : IAESService
    {
        const string IVSETTINGS = "AES:IV";
        private readonly IConfiguration _configuration;

        public AESService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DecryptString(string cipherText, byte[] keyBytes)
        {
            var iv = _configuration[IVSETTINGS];
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            return DecryptString(cipherText, keyBytes, ivBytes);
        }

        public string DecryptString(string cipherText, byte[] keyBytes, byte[] ivBytes)
        {
            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keyBytes, ivBytes);
            return decriptedFromJavascript;
        }

        public string DecryptStringFromBytes(byte[] cipherText, byte[] keyBytes, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (keyBytes == null || keyBytes.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }


            string plaintext = null;

            using (var rijAlg = new AesManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = keyBytes;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public byte[] EncryptStringToBytes(string plainText, byte[] keyBytes)
        {
            var iv = _configuration[IVSETTINGS];
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            return EncryptStringToBytes(plainText, keyBytes, ivBytes);
        }

        public byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }
            byte[] encrypted;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
    }
}
