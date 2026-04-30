using EasySave.Models;
using EasySave.Services;

namespace EasySave.Tests.Services
{
    public class EngineFaultToleranceTests
    {
        // Tests the engine's error handling by simulating a backup execution with a non-existent source directory, ensuring it stops gracefully without throwing a fatal exception.
        [Fact]
        public void ExecuteJob_ShouldNotCrash_WhenSourceDirectoryDoesNotExist()
        {
            var dummyConfig = new AppConfig();
            var engine = new BackupEngine(dummyConfig);

            string fakeSource = "Z:\\FolderThatDoesNotExist_123456";
            var job = new BackupJob("ImpossibleJob", fakeSource, "C:\\Dest", BackupType.Full);

            var exception = Record.Exception(() => engine.ExecuteJob(job));

            Assert.Null(exception);
        }
    }
}