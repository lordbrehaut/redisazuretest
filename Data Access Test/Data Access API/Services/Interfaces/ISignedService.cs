namespace DataAccessAPI.Services.Interfaces
{
    public interface ISignedService
    {
        byte[] GenerateHash(byte[] content);
        bool Verify(byte[] content, out int sentTagLength, int ivLength = 0);
    }
}
