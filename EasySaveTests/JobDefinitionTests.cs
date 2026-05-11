using EasySave.Models;
using Xunit;

namespace EasySaveApp.Tests.Models
{
    public class JobDefinitionTests
    {
        // Verifies that the BackupJob constructor correctly initializes all properties 
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