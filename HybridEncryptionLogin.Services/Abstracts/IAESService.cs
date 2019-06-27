namespace HybridEncryptionLogin.Services.Abstracts
{
    public interface IAESService
    {
        string DecryptString(string cipherText, byte[] keyBytes);
        string DecryptString(string cipherText, byte[] keyBytes, byte[] ivBytes);
        string DecryptStringFromBytes(byte[] cipherText, byte[] keyBytes, byte[] ivBytes);
        byte[] EncryptStringToBytes(string plainText, byte[] keyBytes);
        byte[] EncryptStringToBytes(string plainText, byte[] keyBytes, byte[] ivBytes);
    }
}
