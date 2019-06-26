using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HybridEncryptionLogin.Utils
{
    public class RSAUtils
    {
        public static byte[] DecryptStringRSA(string cipherText, string privateKey)
        {
            byte[] cipherTextData = Convert.FromBase64String(cipherText);

            RSACryptoServiceProvider provider = PemKeyUtils.GetRSAProviderFromPEM(privateKey);
            return provider.Decrypt(cipherTextData, false);
        }
    }
}
