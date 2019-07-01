using System.Security.Cryptography;

namespace HybridEncryptionLogin.Services.Abstracts
{
    public interface IRSAService
    {
        byte[] DecryptString(string cipherText, string pemPrivateKey);
        RSA GetRSAProviderFromPEMPrivate(string pem);
        RSA GetRSAProviderFromPEMPublic(string pem);
        string GetPrivatePEM(RSA csp);
        string GetPublicPEM(RSA csp);
    }
}
