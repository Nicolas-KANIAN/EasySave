using EasySave.Services;

namespace EasySave.Tests.Services
{
    public class EncryptionServiceTests
    {
        [Fact]
        public void Encrypt_ShouldReturnEncryptionTime_WhenFileIsValid()
        {
            var service = new EncryptionService();
            string dummyKey = "MaCleSecrete";

            string dummyFilePath = "fichier_test_crypto.txt";

            File.WriteAllText(dummyFilePath, "Contenu de test pour voir si le chiffrement fonctionne.");

            try
            {
                long result = service.Encrypt(dummyFilePath, dummyKey);

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