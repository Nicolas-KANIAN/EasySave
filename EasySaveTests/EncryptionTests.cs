using EasySave.Services;
namespace EasySave.Tests.Services
{
    public class EncryptionServiceTests
    {
        [Fact]
        public void Encrypt_ShouldReturnEncryptionTime_WhenFileIsValid()
        {
            var service = new EncryptionService();
            string dummyKey = "MySecretKey";

            // Generate a unique temporary file path
            string dummyFilePath = Path.GetTempFileName();

            File.WriteAllText(dummyFilePath, "Test content to see if encryption works.");

            try
            {
                long result = service.Encrypt(dummyFilePath, dummyKey);

                Assert.True(result >= 0, $"CryptoSoft failed with code: {result}");
            }
            finally
            {
                if (File.Exists(dummyFilePath))
                {
                    File.Delete(dummyFilePath);
                }
            }
        }
    }
}