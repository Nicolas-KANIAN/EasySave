using EasySave.Models;
using EasySave.Services;

// Updated namespace for the Avalonia app tests
namespace EasySaveApp.Tests.Services
{
    public class JobManagerTests : IDisposable
    {
        private readonly string _testFilePath;

        public JobManagerTests()
        {
            _testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jobs.json");
            CleanUp();
        }

        public void Dispose()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        // Validates that a new backup job is successfully added to the internal list.
        [Fact]
        public void CreateJob_ShouldAddJob()
        {
            var manager = new JobManager();
            manager.Jobs.Clear();

            manager.CreateJob(new BackupJob("TestJob", "C:\\Src", "D:\\Dest", BackupType.Full));

            Assert.Single(manager.Jobs);
        }

        // The 5-job limit has been removed, so creating 6 jobs should work perfectly.
        [Fact]
        public void CreateJob_ShouldAllowMoreThanFiveJobs()
        {
            var manager = new JobManager();
            manager.Jobs.Clear();

            for (int i = 0; i < 6; i++)
            {
                manager.CreateJob(new BackupJob($"Job{i}", "C:\\Src", "D:\\Dest", BackupType.Full));
            }

            Assert.Equal(6, manager.Jobs.Count);
        }

        [Fact]
        public void DeleteJob_ShouldRemoveJob_WhenIndexIsValid()
        {
            var manager = new JobManager();
            manager.Jobs.Clear();
            manager.CreateJob(new BackupJob("To Delete", "C:\\Src", "D:\\Dest", BackupType.Full));

            manager.DeleteJob(0);

            Assert.Empty(manager.Jobs);
        }

        // Verifies the system's robustness by ensuring no application crash occurs when attempting to delete a job using an out-of-bounds or invalid index.
        [Fact]
        public void DeleteJob_ShouldNotCrash_WhenIndexIsInvalid()
        {
            var manager = new JobManager();
            manager.Jobs.Clear();
            manager.CreateJob(new BackupJob("Job1", "C:\\Src", "D:\\Dest", BackupType.Full));

            var exception1 = Record.Exception(() => manager.DeleteJob(-1));
            var exception2 = Record.Exception(() => manager.DeleteJob(99));

            Assert.Null(exception1);
            Assert.Null(exception2);
            Assert.Single(manager.Jobs);
        }
    }
}