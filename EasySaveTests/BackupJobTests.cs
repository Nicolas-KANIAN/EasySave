using Xunit;
using EasySave.Models;

namespace EasySave.Tests
{
    public class BackupJobTests
    {
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