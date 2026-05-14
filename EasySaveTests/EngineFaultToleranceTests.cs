using EasySave.Models;
using EasySave.Services;

namespace EasySave.Tests.Services
{
    public class EngineFaultToleranceTests
    {
        // Tests the engine's error handling by simulating a backup execution with a non-existent source directory.
        [Fact]
        public void ExecuteJob_ShouldNotCrash_WhenSourceDirectoryDoesNotExist()
        {
            var dummyConfig = new AppConfig();

            var dummyMonitor = new BusinessSoftwareMonitor();
            var engine = new BackupEngine(dummyConfig, dummyMonitor);

            string fakeSource = "Z:\\FolderThatDoesNotExist_123456";
            var job = new BackupJob("ImpossibleJob", fakeSource, "C:\\Dest", BackupType.Full);

            var exception = Record.Exception(() => engine.ExecuteJob(job));

            Assert.Null(exception);
        }
    }
}