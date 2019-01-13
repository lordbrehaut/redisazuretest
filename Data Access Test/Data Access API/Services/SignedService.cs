using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using System;
using System.IO;
using System.Security.Cryptography;

namespace DataAccessAPI.Services
{
    public class SignedService : ISignedService
    {
        private readonly AuthenticationSettings _authenticationSettings;

        public SignedService(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        public byte[] GenerateHash(byte[] content)
        {
            using (var hmac = new HMACSHA256(_authenticationSettings.AuthKey))
            {
                return hmac.ComputeHash(content);
            }
        }

        public bool Verify(byte[] content, out int hashLength, int ivLength = 0)
        {
            using (var hmac = new HMACSHA256(_authenticationSettings.AuthKey))
            {
                var contentHash = new byte[hmac.HashSize / 8];
                var hashForCompare = hmac.ComputeHash(content, 0, content.Length - contentHash.Length);

                if (content.Length < contentHash.Length + ivLength)
                {
                    hashLength = 0;
                    return false;
                }

                Array.Copy(content, content.Length - contentHash.Length, contentHash, 0, contentHash.Length);

                var compare = 0;
                for (var i = 0; i < contentHash.Length; i++)
                    compare |= contentHash[i] ^ hashForCompare[i];

                hashLength = contentHash.Length;
                return compare == 0;
            }
        }
    }
}
