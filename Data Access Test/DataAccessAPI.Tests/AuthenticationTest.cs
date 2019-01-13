using DataAccessAPI.Services;
using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessAPI.Tests
{
    public class AuthenticationTest
    {
        private ISignedService signedService;

        [SetUp]
        public void Setup()
        {
            var authKey = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(authKey);

            signedService = new SignedService(new AuthenticationSettings { AuthKey = authKey });
        }

        [Test]
        public void DoesSign()
        {
            byte[] plainText = Convert.FromBase64String("SamplePlaintext1");
            string hash = Encoding.UTF8.GetString(signedService.GenerateHash(plainText));
            Assert.AreNotEqual(plainText, hash);
        }

        [Test]
        public void DoesSignAndVerify()
        {
            byte[] plainText = Convert.FromBase64String("SamplePlaintext1");
            byte[] hash = signedService.GenerateHash(plainText);

            var signedPlaintext = new byte[plainText.Length + hash.Length];
            Buffer.BlockCopy(plainText, 0, signedPlaintext, 0, plainText.Length);
            Buffer.BlockCopy(hash, 0, signedPlaintext, plainText.Length, hash.Length);

            Assert.IsTrue(signedService.Verify(signedPlaintext, out int sendTagLength));
        }
    }
}
