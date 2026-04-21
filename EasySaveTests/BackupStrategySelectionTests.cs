using Xunit;
using EasySave.Models;
using EasySave.Patterns.Factory;
using EasySave.Patterns.Strategy;

namespace EasySave.Tests.Patterns
{
    public class BackupStrategySelectionTests
    {
        // Ensures that the factory returns a FullBackupStrategy instance when the Full backup type is requested.
        [Fact]
        public void CreateStrategy_ShouldReturnFullStrategy_WhenTypeIsFull()
        {
            var strategy = BackupFactory.CreateStrategy(BackupType.Full);

            Assert.IsType<FullBackupStrategy>(strategy);
        }

        // Ensures that the factory returns a DifferentialBackupStrategy instance when the Differential backup type is requested.
        [Fact]
        public void CreateStrategy_ShouldReturnDiffStrategy_WhenTypeIsDiff()
        {
            var strategy = BackupFactory.CreateStrategy(BackupType.Differential);

            Assert.IsType<DifferentialBackupStrategy>(strategy);
        }
    }
}