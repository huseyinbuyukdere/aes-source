using AesSource;
using NUnit.Framework;
using System;



namespace AesSourceTest
{
    [TestFixture]
    public class AesSourceTests
    {
        AesController aesController;

        [SetUp]
        public void Setup()
        {
            aesController = new AesController();
        }

        [Test]
        public void Encrypt_WhenSentPlainText_ReturnsEncryptedText()
        {
            var genericResponse = aesController.Encrypt("Two One Nine Two", "Thats my Kung Fu");
    
            Assert.IsTrue(genericResponse.IsSuccess);
        }

        [Test]
        public void Decrypt_WhenSentEncryptedText_ReturnsDecryptedText()
        {
            var genericResponse = aesController.Decrypt("KcNQX1cUIPZAIpmzGgLXOg==", "Thats my Kung Fu");

            Assert.IsTrue(genericResponse.IsSuccess);
        }
    }
}
