using EasySave.Services;

namespace EasySave.Tests.Services
{
    public class EncryptionServiceTests
    {
        [Fact]
        public void Encrypt_ShouldReturnEncryptionTime_WhenFileIsValid()
        {
            var dummyConfig = new AppConfig();

            var service = new EncryptionService(dummyConfig);

            string dummyKey = "MySecretKey";

            string dummyFilePath = Path.GetTempFileName();
            File.WriteAllText(dummyFilePath, "Test content to see if encryption works.");

            try
            {
                long result = service.Encrypt(dummyFilePath, dummyKey);

                Assert.NotEqual(-2, result);
                Assert.True(result >= 0, $"CryptoSoft a échoué avec le code : {result}");
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