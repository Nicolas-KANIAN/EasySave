using EasySave.Models;
using EasySave.Patterns.Factory;
using EasySave.Patterns.Strategy;

namespace EasySave.Tests.Patterns
{
    public class BackupStrategySelectionTests
    {
        // Ensures that the factory correctly instantiates and returns a FullBackupStrategy 
        [Fact]
        public void CreateStrategy_ShouldReturnFullStrategy_WhenTypeIsFull()
        {
            var strategy = BackupFactory.CreateStrategy(BackupType.Full);

            Assert.IsType<FullBackupStrategy>(strategy);
        }

        // Ensures that the factory correctly instantiates and returns a DifferentialBackupStrategy 
        [Fact]
        public void CreateStrategy_ShouldReturnDiffStrategy_WhenTypeIsDiff()
        {
            var strategy = BackupFactory.CreateStrategy(BackupType.Differential);

            Assert.IsType<DifferentialBackupStrategy>(strategy);
        }
    }
}