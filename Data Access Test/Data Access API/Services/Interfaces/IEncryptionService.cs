namespace DataAccessAPI.Services.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string secretMessage);
        string Decrypt(string encryptedMessage);
    }
}