using EasySave.Models;
using EasySave.Services;

namespace EasySave.Tests.Services
{
    public class JobLimitRulesTests
    {
        private void CleanUp()
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "jobs.json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        // Validates that a new backup job is successfully added to the internal list when the maximum limit is not yet reached.
        [Fact]
        public void CreateJob_ShouldAddJob_WhenLimitNotReached()
        {
            CleanUp();
            var manager = new JobManager();
            manager.Jobs.Clear();

            manager.CreateJob(new BackupJob("TestJob", "C:\\Src", "D:\\Dest", BackupType.Full));

            Assert.Single(manager.Jobs);
            CleanUp();
        }

        // Tests the strict business rule that prevents the creation of more than 5 backup jobs simultaneously.
        [Fact]
        public void CreateJob_ShouldNotExceedLimitOfFive()
        {
            CleanUp();
            var manager = new JobManager();
            manager.Jobs.Clear();

            for (int i = 0; i < 6; i++)
            {
                manager.CreateJob(new BackupJob($"Job{i}", "C:\\Src", "D:\\Dest", BackupType.Full));
            }

            Assert.Equal(5, manager.Jobs.Count);
            CleanUp();
        }

        // Ensures that an existing backup job is successfully removed from the list when a valid index is provided.
        [Fact]
        public void DeleteJob_ShouldRemoveJob_WhenIndexIsValid()
        {
            CleanUp();
            var manager = new JobManager();
            manager.Jobs.Clear();
            manager.CreateJob(new BackupJob("To Delete", "C:\\Src", "D:\\Dest", BackupType.Full));

            manager.DeleteJob(0);

            Assert.Empty(manager.Jobs);
            CleanUp();
        }

        // Verifies the system's robustness by ensuring no application crash occurs when attempting to delete a job using an out-of-bounds or invalid index.
        [Fact]
        public void DeleteJob_ShouldNotCrash_WhenIndexIsInvalid()
        {
            CleanUp();
            var manager = new JobManager();
            manager.Jobs.Clear();
            manager.CreateJob(new BackupJob("Job1", "C:\\Src", "D:\\Dest", BackupType.Full));

            var exception1 = Record.Exception(() => manager.DeleteJob(-1));
            var exception2 = Record.Exception(() => manager.DeleteJob(99));

            Assert.Null(exception1);
            Assert.Null(exception2);
            Assert.Single(manager.Jobs);
            CleanUp();
        }
    }
}