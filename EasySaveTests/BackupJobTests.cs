/**********************************
*
* La classe BackupJobTests permet de vérifier que la classe
* BackupJob focntionne correctement.
*
**********************************/

using EasySave.Models;
using Xunit;

namespace EasySave.Tests
{
    public class BackupJobTests
    {

        /**********************
        *
        *   Ce test vérifie que le constructeur de BackupJob 
        *   affecte les propriétés correctement.
        *   
        *   Si les valeurs de l'objet sont celles initialisé dans
        *   le test, alors le constructeur fonctionne.
        *
        **********************/
        [Fact]
        public void BackupJob_Creation_ShouldSetPropertiesCorrectly()
        {
            string expectedName = "TestJob";
            string expectedSource = @"C:\Source";
            string expectedTarget = @"D:\Target";
            BackupType expectedType = BackupType.Full;

            BackupJob job = new BackupJob(expectedName, expectedSource, expectedTarget, expectedType);

            Assert.Equal(expectedName, job.Name);
            Assert.Equal(expectedSource, job.SourceDirectory);
            Assert.Equal(expectedTarget, job.TargetDirectory);
            Assert.Equal(expectedType, job.Type);
        }
    }
}