using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HybridEncryptionLogin.Services.Abstracts
{
    public interface IRSAService
    {
        byte[] DecryptString(string cipherText, string pemPrivateKey);
        RSACryptoServiceProvider GetRSAProviderFromPEMPrivate(string pem);
        RSACryptoServiceProvider GetRSAProviderFromPEMPublic(string pem);
        string GetPrivatePEM(RSACryptoServiceProvider csp);
        string GetPublicPEM(RSACryptoServiceProvider csp);
    }
}
