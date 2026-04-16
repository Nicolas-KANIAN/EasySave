using Xunit;
using EasySave.Models; // On importe ton modèle

namespace EasySave.Tests
{
    public class BackupJobTests
    {
        [Fact] // Cette balise indique à Visual Studio que la méthode en dessous est un test unitaire
        public void BackupJob_Creation_ShouldSetPropertiesCorrectly()
        {
            // 1. Arrange (Préparation des données)
            string expectedName = "TestJob";
            string expectedSource = @"C:\Source";
            string expectedTarget = @"D:\Target";
            BackupType expectedType = BackupType.Full;

            // 2. Act (L'action qu'on veut tester)
            BackupJob job = new BackupJob(expectedName, expectedSource, expectedTarget, expectedType);

            // 3. Assert (Les vérifications)
            Assert.Equal(expectedName, job.Name);
            Assert.Equal(expectedSource, job.SourceDirectory);
            Assert.Equal(expectedTarget, job.TargetDirectory);
            Assert.Equal(expectedType, job.Type);
        }
    }
}