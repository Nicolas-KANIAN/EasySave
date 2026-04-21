using Xunit;
using EasyLog;

namespace EasySave.Tests.EasyLogTests
{
    public class LogFileUniquenessTests
    {
        // Verifies the Singleton pattern implementation. It ensures that multiple calls to the Logger instance always return the exact same object in memory, preventing file access conflicts.
        [Fact]
        public void LoggerInstance_ShouldBeASingleton_AndReturnSameObject()
        {
            var logger1 = Logger.Instance;
            var logger2 = Logger.Instance;

            Assert.Same(logger1, logger2);
        }
    }
}