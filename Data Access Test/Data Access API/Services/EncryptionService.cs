using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessAPI.Services
{
    public class EncryptionService : IEncryptionService
    {
        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

        private const int BlockBitSize = 128;
        private const int KeyBitSize = 256;
        private const int SaltBitSize = 64;
        private const int Iterations = 10000;
        private const int MinPasswordLength = 12;

        private readonly EncryptionSettings _encryptionSettings;
        private readonly ISignedService _signedService;

        public EncryptionService(EncryptionSettings encryptionSettings, ISignedService signedService)
        {
            _encryptionSettings = encryptionSettings;
            _signedService = signedService;
        }
        
        public string Encrypt(string secretMessage)
        {
            var plainText = Encoding.UTF8.GetBytes(secretMessage);
            var cipherText = SimpleEncrypt(plainText);
            return Convert.ToBase64String(cipherText);
        }

        public string Decrypt(string encryptedMessage)
        {
            if (string.IsNullOrWhiteSpace(encryptedMessage))
                throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

            var cipherText = Convert.FromBase64String(encryptedMessage);
            var plainText = SimpleDecrypt(cipherText);
            return plainText == null ? null : Encoding.UTF8.GetString(plainText);
        }

        public byte[] SimpleEncrypt(byte[] secretMessage)
        {
            byte[] cipherText;
            byte[] iv;

            using (var aes = new AesManaged
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                aes.GenerateIV();
                iv = aes.IV;

                using (var encrypter = aes.CreateEncryptor(_encryptionSettings.CryptKey, iv))
                using (var cipherStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        binaryWriter.Write(secretMessage);
                    }

                    cipherText = cipherStream.ToArray();
                }
            }

            using (var encryptedStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(encryptedStream))
                {
                    binaryWriter.Write(iv);
                    binaryWriter.Write(cipherText);
                    binaryWriter.Flush();

                    var tag = _signedService.GenerateHash(encryptedStream.ToArray());
                    binaryWriter.Write(tag);
                }
                return encryptedStream.ToArray();
            }
        }

        public byte[] SimpleDecrypt(byte[] encryptedMessage)
        {
            var cryptKey = _encryptionSettings.CryptKey;

            var ivLength = (BlockBitSize / 8);
            if (!_signedService.Verify(encryptedMessage, out int tagLength, ivLength))
            {
                return null;
            }

            using (var aes = new AesManaged {
                    KeySize = KeyBitSize,
                    BlockSize = BlockBitSize,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
            })
            {

                var iv = new byte[ivLength];
                Array.Copy(encryptedMessage, 0, iv, 0, iv.Length);

                using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
                using (var plainTextStream = new MemoryStream())
                {
                    using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(decrypterStream))
                    {
                        binaryWriter.Write(
                            encryptedMessage,
                            iv.Length,
                            encryptedMessage.Length - iv.Length - tagLength
                        );
                    }
                    return plainTextStream.ToArray();
                }
            }
        }
    }
}
