using DataAccessAPI.Services;
using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using NUnit.Framework;
using System.Security.Cryptography;

namespace DataAccessAPI.Tests
{
    public class EncryptionTest
    {
        private IEncryptionService encryptionService;

        [SetUp]
        public void Setup()
        {
            var authKey = new byte[32];
            var cryptKey = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(authKey);
            new RNGCryptoServiceProvider().GetBytes(cryptKey);

            encryptionService = new EncryptionService(
                new EncryptionSettings { CryptKey = cryptKey }, 
                new SignedService(new AuthenticationSettings { AuthKey = authKey }));
        }

        [Test]
        public void DoesEncrypt()
        {
            string plainText = "SamplePlaintext1";
            string cipherText = encryptionService.Encrypt(plainText);
            Assert.AreNotEqual(plainText, cipherText);
        }

        [Test]
        public void DoesEncryptAndDecryptCorrectly()
        {
            string plainText = "SamplePlaintext1";
            string cipherText = encryptionService.Encrypt(plainText);
            string plainText2 = encryptionService.Decrypt(cipherText);
            Assert.AreEqual(plainText, plainText2);
        }
    }
}